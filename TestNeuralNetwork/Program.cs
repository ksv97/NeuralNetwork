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
			double learningSpeedIncrease = 0.1;
			double alphaIncrease = 0.15;

			double startLearningSpeed = 0.5;
			double startAlpha = 0.5;

			int maxEpoch = 3000;

			Console.WriteLine("Максимальная эпоха нейронной сети = {0}", maxEpoch);
			Console.WriteLine("Параметры эксперимента");
			Console.WriteLine("Коэффициент скорости обучения = [{0}; {1}] с шагом {2}",startLearningSpeed, startLearningSpeed + 6 * learningSpeedIncrease, learningSpeedIncrease);
			Console.WriteLine("Наклон сигмоида = [{0}; {1}] с шагом {2}",startAlpha, startAlpha + 6 * alphaIncrease,  alphaIncrease);
			for (int i = 0; i < 6; i++)
			{
				Console.WriteLine("Эксперимент №{0}", i + 1);

				double learningSpeed = startLearningSpeed + i * learningSpeedIncrease;
				double alpha = startAlpha + i * alphaIncrease;
				int neuronsCount = 15;
				//double epsilon = 0.155;
				NeuralNetwork neuralNetwork = new NeuralNetwork(learningSpeed, alpha, neuronsCount, maxEpoch);

				neuralNetwork.CreateNetwork();

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				neuralNetwork.Train(testData);
				stopwatch.Stop();				
				Console.WriteLine("Экспериментальные параметры нейронной сети:");
				Console.WriteLine("Коэффициент скорости обучения = {0}, наклон сигмоида = {1}", learningSpeed, alpha);
				Console.WriteLine("Результат Root MSE для НС = {0}", neuralNetwork.Error);

				Console.WriteLine("Результаты обучения в милисекундах: {0}", stopwatch.ElapsedMilliseconds);
			}

			Console.WriteLine("Экспериментальные данные успешно сохранены в файл experiment.dat");
			Console.ReadKey();

			

			
			//List<double> input = new List<double> { 1, 0 };
			//GetResultFromTeachedNetwork(neuralNetwork, input);

			//char input;
			//do
			//{
			//	Console.WriteLine("Choose inputs:");
			//	Console.WriteLine("1 - [0, 0]");
			//	Console.WriteLine("2 - [0, 1]");
			//	Console.WriteLine("3 - [1, 0]");
			//	Console.WriteLine("4 - [1, 1]");
			//	Console.WriteLine("0 - exit application");
			//	input = (Console.ReadKey()).KeyChar;
			//	switch (input)
			//	{
			//		case '1': neuralNetwork.Handle(new List<double> { 0, 0 }); WriteResult(neuralNetwork); break;
			//		case '2': neuralNetwork.Handle(new List<double> { 0, 1 }); WriteResult(neuralNetwork); break;
			//		case '3': neuralNetwork.Handle(new List<double> { 1, 0 }); WriteResult(neuralNetwork); break;
			//		case '4': neuralNetwork.Handle(new List<double> { 1, 1 }); WriteResult(neuralNetwork); break;
			//		case '0': return;
			//	}
			//} while (input != '0');
			//neuralNetwork.Handle(new List<double> { 0, 1 });
			
		}

		static void GetResultFromTeachedNetwork(NeuralNetwork network, List<double> input)
		{
			Console.WriteLine("\nВычисление логических функций обученной сети при входных данных ({0},{1})", input[0], input[1]);
			network.Handle(input);
			WriteResult(network);
		}

		static void WriteResult (NeuralNetwork network)
		{			
			Console.WriteLine("Результаты для следующих функций: ^ V + <-> ->: ");
			foreach (Neuron neuron in network.Neurons[2])
			{ 				
				Console.Write($"{(neuron.Output)} ");
			}
			Console.WriteLine("\nНажмите для продолжения");
			Console.ReadKey();
			Console.Clear();
		}
	}
}
