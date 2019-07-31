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
			/*
				|   Method |        Mean |     Error |    StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
				|--------- |------------:|----------:|----------:|--------:|-------:|------:|----------:|
				|       NJ |    62.33 us |  1.214 us |  1.247 us |  5.6152 |      - |     - |  11.63 KB |
				| STJAsync | 1,128.95 us | 22.195 us | 22.793 us |  9.7656 | 3.9063 |     - |  20.99 KB |
				|      STJ | 1,063.56 us | 17.263 us | 16.148 us | 13.6719 | 5.8594 |     - |   29.4 KB |
			 */
		}
	}
}
