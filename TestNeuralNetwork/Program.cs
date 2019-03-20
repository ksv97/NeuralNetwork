using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TestNeuralNetwork
{
	class Program
	{


		static void Main(string[] args)
		{
			Data[] testData = new Data[]
			{

				new Data
				{
					Inputs = {0, 0 },
					Outputs = {0, 0, 0, 1, 1 }

				},
				new Data
				{
					Inputs = {0, 1 },
					Outputs = {0, 1, 1, 0, 1 }
				},
				new Data
				{
					Inputs = {1, 0 },
					Outputs = {0, 1, 1, 0, 0 }
				},
				new Data
				{
					Inputs = {1,1},
					Outputs = {1, 1, 0, 1, 1 }
				}
			};

			// double speedOfDecreasingLearingSpeed
			double learningSpeed = 0.7;
			double alpha = 1.5;
			int neuronsCount = 15;			
			NeuralNetwork neuralNetwork = new NeuralNetwork(learningSpeed, alpha, neuronsCount);

			neuralNetwork.CreateNetwork();

			Console.WriteLine("Training in progress");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			neuralNetwork.Train(testData);
			stopwatch.Stop();
			Console.Clear();
			Console.WriteLine("Training completed!");
			Console.WriteLine("Miliseconds spent for training: {0}", stopwatch.ElapsedMilliseconds);

			char input;
			do
			{
				Console.WriteLine("Choose inputs:");
				Console.WriteLine("1 - [0, 0]");
				Console.WriteLine("2 - [0, 1]");
				Console.WriteLine("3 - [1, 0]");
				Console.WriteLine("4 - [1, 1]");
				Console.WriteLine("0 - exit application");
				input = (Console.ReadKey()).KeyChar;
				switch (input)
				{
					case '1': neuralNetwork.Handle(new List<double> { 0, 0 }); WriteResult(neuralNetwork); break;
					case '2': neuralNetwork.Handle(new List<double> { 0, 1 }); WriteResult(neuralNetwork); break;
					case '3': neuralNetwork.Handle(new List<double> { 1, 0 }); WriteResult(neuralNetwork); break;
					case '4': neuralNetwork.Handle(new List<double> { 1, 1 }); WriteResult(neuralNetwork); break;
					case '0': return;
				}
			} while (input != '0');
			//neuralNetwork.Handle(new List<double> { 0, 1 });
			
		}

		static void WriteResult (NeuralNetwork network)
		{

			Console.WriteLine("\nInputs: ");
			foreach (Neuron neuron in network.Neurons[0])
			{
				Console.Write($"{neuron.Output} ");
			}
			Console.WriteLine("\nResult for logical functions: ^ V + <-> ->: ");
			foreach (Neuron neuron in network.Neurons[2])
			{ 				
				Console.Write($"{/*Math.Round*/(neuron.Output)} ");
			}
			Console.WriteLine("\nPress any button to continue");
			Console.ReadKey();
			Console.Clear();
		}
	}
}
