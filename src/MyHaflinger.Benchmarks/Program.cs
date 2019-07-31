using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MyHaflinger.Benchmarks
{
	class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<NJvsSTJ>();
			/*  netcoreapp3.0 tfm
				|   Method |        Mean |     Error |    StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
				|--------- |------------:|----------:|----------:|--------:|-------:|------:|----------:|
				|       NJ |    62.33 us |  1.214 us |  1.247 us |  5.6152 |      - |     - |  11.63 KB |
				| STJAsync | 1,128.95 us | 22.195 us | 22.793 us |  9.7656 | 3.9063 |     - |  20.99 KB |
				|      STJ | 1,063.56 us | 17.263 us | 16.148 us | 13.6719 | 5.8594 |     - |   29.4 KB |
			 */
			/*  netcoreapp2.2 tfm
				|   Method |     Mean |      Error |     StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
				|--------- |---------:|-----------:|-----------:|-------:|------:|------:|----------:|
				|       NJ | 217.7 us | 25.8898 us | 73.4452 us |      - |     - |     - |  11.68 KB |
				| STJAsync | 235.0 us |  0.7094 us |  0.6636 us | 5.8594 |     - |     - |  10.61 KB |
				|      STJ | 103.5 us |  1.3205 us |  1.1027 us | 9.2773 |     - |     - |  19.18 KB |
			 */
		}
	}
}
