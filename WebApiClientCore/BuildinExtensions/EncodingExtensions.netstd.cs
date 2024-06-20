#if NETSTANDARD2_1
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供Encoding扩展
    /// </summary>
    static partial class EncodingExtensions
    {
        /// <summary>
        /// The maximum number of input elements after which we'll begin to chunk the input.
        /// </summary>
        /// <remarks>
        /// The reason for this chunking is that the existing Encoding / Encoder / Decoder APIs
        /// like GetByteCount / GetCharCount will throw if an integer overflow occurs. Since
        /// we may be working with large inputs in these extension methods, we don't want to
        /// risk running into this issue. While it's technically possible even for 1 million
        /// input elements to result in an overflow condition, such a scenario is unrealistic,
        /// so we won't worry about it.
        /// </remarks>
        private const int MaxInputElementsPerIteration = 1 * 1024 * 1024;

        /// <summary>
        /// Encodes the specified <see cref="ReadOnlySpan{Char}"/> to <see langword="byte"/>s using the specified <see cref="Encoding"/>
        /// and writes the result to <paramref name="writer"/>.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> which represents how the data in <paramref name="chars"/> should be encoded.</param>
        /// <param name="chars">The <see cref="ReadOnlySpan{Char}"/> to encode to <see langword="byte"/>s.</param>
        /// <param name="writer">The buffer to which the encoded bytes will be written.</param>
        /// <exception cref="EncoderFallbackException">Thrown if <paramref name="chars"/> contains data that cannot be encoded and <paramref name="encoding"/> is configured
        /// to throw an exception when such data is seen.</exception>
        public static long GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, IBufferWriter<byte> writer)
        {
            if (chars.Length <= MaxInputElementsPerIteration)
            {
                // The input span is small enough where we can one-shot this.

                int byteCount = encoding.GetByteCount(chars);
                Span<byte> scratchBuffer = writer.GetSpan(byteCount);

                int actualBytesWritten = encoding.GetBytes(chars, scratchBuffer);

                writer.Advance(actualBytesWritten);
                return actualBytesWritten;
            }
            else
            {
                // Allocate a stateful Encoder instance and chunk this.

                Convert(encoding.GetEncoder(), chars, writer, flush: true, out long totalBytesWritten, out _);
                return totalBytesWritten;
            }
        } 

        /// <summary>
        /// Decodes the specified <see cref="ReadOnlySpan{Byte}"/> to <see langword="char"/>s using the specified <see cref="Encoding"/>
        /// and writes the result to <paramref name="writer"/>.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> which represents how the data in <paramref name="bytes"/> should be decoded.</param>
        /// <param name="bytes">The <see cref="ReadOnlySpan{Byte}"/> whose bytes should be decoded.</param>
        /// <param name="writer">The buffer to which the decoded chars will be written.</param>
        /// <returns>The number of chars written to <paramref name="writer"/>.</returns>
        /// <exception cref="DecoderFallbackException">Thrown if <paramref name="bytes"/> contains data that cannot be decoded and <paramref name="encoding"/> is configured
        /// to throw an exception when such data is seen.</exception>
        public static long GetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, IBufferWriter<char> writer)
        {
            if (bytes.Length <= MaxInputElementsPerIteration)
            {
                // The input span is small enough where we can one-shot this.

                int charCount = encoding.GetCharCount(bytes);
                Span<char> scratchBuffer = writer.GetSpan(charCount);

                int actualCharsWritten = encoding.GetChars(bytes, scratchBuffer);

                writer.Advance(actualCharsWritten);
                return actualCharsWritten;
            }
            else
            {
                // Allocate a stateful Decoder instance and chunk this.

                Convert(encoding.GetDecoder(), bytes, writer, flush: true, out long totalCharsWritten, out _);
                return totalCharsWritten;
            }
        } 

        /// <summary>
        /// Converts a <see cref="ReadOnlySpan{Char}"/> to bytes using <paramref name="encoder"/> and writes the result to <paramref name="writer"/>.
        /// </summary>
        /// <param name="encoder">The <see cref="Encoder"/> instance which can convert <see langword="char"/>s to <see langword="byte"/>s.</param>
        /// <param name="chars">A sequence of characters to encode.</param>
        /// <param name="writer">The buffer to which the encoded bytes will be written.</param>
        /// <param name="flush"><see langword="true"/> to indicate no further data is to be converted; otherwise <see langword="false"/>.</param>
        /// <param name="bytesUsed">When this method returns, contains the count of <see langword="byte"/>s which were written to <paramref name="writer"/>.</param>
        /// <param name="completed">
        /// When this method returns, contains <see langword="true"/> if <paramref name="encoder"/> contains no partial internal state; otherwise, <see langword="false"/>.
        /// If <paramref name="flush"/> is <see langword="true"/>, this will always be set to <see langword="true"/> when the method returns.
        /// </param>
        /// <exception cref="EncoderFallbackException">Thrown if <paramref name="chars"/> contains data that cannot be encoded and <paramref name="encoder"/> is configured
        /// to throw an exception when such data is seen.</exception>
        public static void Convert(this Encoder encoder, ReadOnlySpan<char> chars, IBufferWriter<byte> writer, bool flush, out long bytesUsed, out bool completed)
        {
            // We need to perform at least one iteration of the loop since the encoder could have internal state.

            long totalBytesWritten = 0;

            do
            {
                // If our remaining input is very large, instead truncate it and tell the encoder
                // that there'll be more data after this call. This truncation is only for the
                // purposes of getting the required byte count. Since the writer may give us a span
                // larger than what we asked for, we'll pass the entirety of the remaining data
                // to the transcoding routine, since it may be able to make progress beyond what
                // was initially computed for the truncated input data.

                int byteCountForThisSlice = (chars.Length <= MaxInputElementsPerIteration)
                  ? encoder.GetByteCount(chars, flush)
                  : encoder.GetByteCount(chars.Slice(0, MaxInputElementsPerIteration), flush: false /* this isn't the end of the data */);

                Span<byte> scratchBuffer = writer.GetSpan(byteCountForThisSlice);

                encoder.Convert(chars, scratchBuffer, flush, out int charsUsedJustNow, out int bytesWrittenJustNow, out completed);

                chars = chars.Slice(charsUsedJustNow);
                writer.Advance(bytesWrittenJustNow);
                totalBytesWritten += bytesWrittenJustNow;
            } while (!chars.IsEmpty);

            bytesUsed = totalBytesWritten;
        }
         

        /// <summary>
        /// Converts a <see cref="ReadOnlySpan{Byte}"/> to chars using <paramref name="decoder"/> and writes the result to <paramref name="writer"/>.
        /// </summary>
        /// <param name="decoder">The <see cref="Decoder"/> instance which can convert <see langword="byte"/>s to <see langword="char"/>s.</param>
        /// <param name="bytes">A sequence of bytes to decode.</param>
        /// <param name="writer">The buffer to which the decoded chars will be written.</param>
        /// <param name="flush"><see langword="true"/> to indicate no further data is to be converted; otherwise <see langword="false"/>.</param>
        /// <param name="charsUsed">When this method returns, contains the count of <see langword="char"/>s which were written to <paramref name="writer"/>.</param>
        /// <param name="completed">
        /// When this method returns, contains <see langword="true"/> if <paramref name="decoder"/> contains no partial internal state; otherwise, <see langword="false"/>.
        /// If <paramref name="flush"/> is <see langword="true"/>, this will always be set to <see langword="true"/> when the method returns.
        /// </param>
        /// <exception cref="DecoderFallbackException">Thrown if <paramref name="bytes"/> contains data that cannot be encoded and <paramref name="decoder"/> is configured
        /// to throw an exception when such data is seen.</exception>
        public static void Convert(this Decoder decoder, ReadOnlySpan<byte> bytes, IBufferWriter<char> writer, bool flush, out long charsUsed, out bool completed)
        {
            // We need to perform at least one iteration of the loop since the decoder could have internal state.

            long totalCharsWritten = 0;

            do
            {
                // If our remaining input is very large, instead truncate it and tell the decoder
                // that there'll be more data after this call. This truncation is only for the
                // purposes of getting the required char count. Since the writer may give us a span
                // larger than what we asked for, we'll pass the entirety of the remaining data
                // to the transcoding routine, since it may be able to make progress beyond what
                // was initially computed for the truncated input data.

                int charCountForThisSlice = (bytes.Length <= MaxInputElementsPerIteration)
                    ? decoder.GetCharCount(bytes, flush)
                    : decoder.GetCharCount(bytes.Slice(0, MaxInputElementsPerIteration), flush: false /* this isn't the end of the data */);

                Span<char> scratchBuffer = writer.GetSpan(charCountForThisSlice);

                decoder.Convert(bytes, scratchBuffer, flush, out int bytesUsedJustNow, out int charsWrittenJustNow, out completed);

                bytes = bytes.Slice(bytesUsedJustNow);
                writer.Advance(charsWrittenJustNow);
                totalCharsWritten += charsWrittenJustNow;
            } while (!bytes.IsEmpty);

            charsUsed = totalCharsWritten;
        } 
    }
}

#endif