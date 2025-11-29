#!/bin/bash

# Benchmark Results Summary Script
# 用于从 BenchmarkDotNet JSON 结果中提取关键性能指标

echo "# Benchmark Results Summary"
echo ""
echo "## Multi-Framework Performance Comparison"
echo ""

# 定义框架列表
frameworks=("netcoreapp3.1" "net5.0" "net8.0" "net10.0" "net10.0-aot")

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
                
                # 使用 jq 提取关键指标（如果可用）
                if command -v jq &> /dev/null; then
                    echo "| Method | Mean | Allocated |"
                    echo "|--------|------|-----------|"
                    
                    jq -r '.Benchmarks[] | "| \(.Method) | \(.Statistics.Mean // "N/A") ns | \(.Memory.BytesAllocatedPerOperation // "N/A") B |"' "$json_file" 2>/dev/null || echo "Unable to parse JSON"
                else
                    echo "jq not available, showing file location: $json_file"
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

echo "## Notes"
echo ""
echo "- Mean times are in nanoseconds (ns)"
echo "- Allocated memory is in bytes (B)"
echo "- Download the artifact 'benchmark-results' for detailed reports"
echo ""
