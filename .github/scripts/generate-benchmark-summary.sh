#!/bin/bash

# Benchmark Results Summary Script
# 直接从 BenchmarkDotNet 生成的 Markdown 报告中提取结果

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
        
        # 查找 Markdown 报告文件
        md_files=$(find "$result_dir" -name "*-report-github.md" -type f 2>/dev/null)
        
        if [ -n "$md_files" ]; then
            for md_file in $md_files; do
                benchmark_name=$(basename "$md_file" -report-github.md)
                echo "#### $benchmark_name"
                echo ""
                
                # 直接提取 Markdown 表格（从表头到空行）
                # 查找包含 "| Method" 的行，然后提取完整表格
                awk '/\| Method.*\|/ {found=1} found {print} /^$/ && found {exit}' "$md_file" | 
                    grep -v "^$" || echo "No table found in $md_file"
                
                echo ""
            done
        else
            echo "No Markdown reports found in $result_dir"
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
echo "- **Mean**: Average execution time"
echo "- **Error**: Half of 99.9% confidence interval"
echo "- **StdDev**: Standard deviation of all measurements"
echo "- **Ratio**: Performance ratio compared to baseline (lower is better for baseline)"
echo "- **RatioSD**: Standard deviation of the Ratio"
echo "- **Gen0**: GC Generation 0 collections per 1000 operations"
echo "- **Allocated**: Total memory allocated per operation"
echo "- **Alloc Ratio**: Memory allocation ratio compared to baseline"
echo ""
echo "## Notes"
echo ""
echo "- **Baseline** method (marked with 🏆 or Ratio=1.00) is the reference point"
echo "- Lower values are better for Mean, Error, StdDev, and Allocated"
echo "- Ratio > 1.00 means slower/more memory than baseline"
echo "- Download the artifact 'benchmark-results' for detailed reports and raw data"
echo ""
