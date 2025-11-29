#!/bin/bash

# Benchmark Results Summary Script
# 用于从 BenchmarkDotNet JSON 结果中提取关键性能指标

echo "# Benchmark Results Summary"
echo ""
echo "## Multi-Framework Performance Comparison"
echo ""

# 定义框架列表（暂时移除 net10.0-aot，因为 BenchmarkDotNet 不支持 AOT）
frameworks=("netcoreapp3.1" "net5.0" "net8.0" "net10.0")

# 查找所有结果目录
for fw in "${frameworks[@]}"; do
    result_dir="BenchmarkDotNet.Artifacts.${fw}/results"
    
    if [ -d "$result_dir" ]; then
        echo "### Framework: ${fw}"
        echo ""
        
        # 查找 JSON 结果文件
        json_files=$(find "$result_dir" -name "*.json" -type f 2>/dev/null)
        
        if [ -n "$json_files" ]; then
            echo "Found benchmark results:"
            
            for json_file in $json_files; do
                benchmark_name=$(basename "$json_file" .json)
                echo ""
                echo "#### $benchmark_name"
                
                # 使用 jq 提取完整的关键指标（如果可用）
                if command -v jq &> /dev/null; then
                    echo "| Method | Mean (μs) | Error (μs) | StdDev (μs) | Ratio | Gen0 | Allocated (KB) | Alloc Ratio |"
                    echo "|--------|-----------|------------|-------------|-------|------|----------------|-------------|"
                    
                    # 提取详细数据，转换单位
                    jq -r '.Benchmarks[] | 
                        "| \(.Method) | \(
                            if .Statistics.Mean then 
                                (.Statistics.Mean / 1000 | . * 100 | round / 100) 
                            else "N/A" end
                        ) | \(
                            if .Statistics.StandardError then 
                                (.Statistics.StandardError / 1000 | . * 100 | round / 100) 
                            else "N/A" end
                        ) | \(
                            if .Statistics.StandardDeviation then 
                                (.Statistics.StandardDeviation / 1000 | . * 100 | round / 100) 
                            else "N/A" end
                        ) | \(
                            if .Ratio then 
                                (.Ratio | . * 100 | round / 100)
                            else "N/A" end
                        ) | \(
                            .Memory.Gen0Collections // "0"
                        ) | \(
                            if .Memory.BytesAllocatedPerOperation then 
                                (.Memory.BytesAllocatedPerOperation / 1024 | . * 100 | round / 100) 
                            else "N/A" end
                        ) | \(
                            if .Memory.AllocRatio then 
                                (.Memory.AllocRatio | . * 100 | round / 100)
                            else "N/A" end
                        ) |"' "$json_file" 2>/dev/null || echo "Unable to parse JSON"
                else
                    echo "⚠️ jq not available, showing file location: $json_file"
                    echo ""
                    echo "Please install jq to see detailed performance metrics."
                fi
            done
        else
            echo "No JSON results found in $result_dir"
        fi
        
        echo ""
        echo "---"
        echo ""
    else
        echo "### Framework: ${fw} - **No results found**"
        echo ""
    fi
done

echo "## Performance Metrics Explanation"
echo ""
echo "- **Mean**: Average execution time in microseconds (μs)"
echo "- **Error**: Half of 99.9% confidence interval"
echo "- **StdDev**: Standard deviation of all measurements"
echo "- **Ratio**: Mean of current method divided by baseline mean"
echo "- **Gen0**: GC Generation 0 collections per 1000 operations"
echo "- **Allocated**: Total memory allocated per operation in kilobytes (KB)"
echo "- **Alloc Ratio**: Allocated memory ratio compared to baseline"
echo ""
echo "## Notes"
echo ""
echo "- Lower values are better for Mean, Error, StdDev, and Allocated"
echo "- Ratio of 1.00 means equal performance to baseline (typically the first method)"
echo "- Download the artifact 'benchmark-results' for detailed reports and raw JSON data"
echo ""
