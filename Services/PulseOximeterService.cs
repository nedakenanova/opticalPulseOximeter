using opticalPulse.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace opticalPulse.Services
{
	public class PulseOximeterService
	{
		public PulseOximeterResult Calculate(
			int[] irBuffer,
			int[] redBuffer)
		{
			
			if (irBuffer == null || redBuffer == null)
			{
				return InvalidResult();
			}

			if (irBuffer.Length == 0 || redBuffer.Length == 0)
			{
				return InvalidResult();
			}

			if (irBuffer.Length != redBuffer.Length)
			{
				return InvalidResult();
			}

			double[] irSignal = irBuffer
				.Select(x => (double)x)
				.ToArray();

			double[] redSignal = redBuffer
				.Select(x => (double)x)
				.ToArray();


			

			double irMean = irSignal.Average();

			for (int i = 0; i < irSignal.Length; i++)
			{
				irSignal[i] = irSignal[i] - irMean;
			}


		

			irSignal = MovingAverage4(irSignal);


			

			double[] difference = CalculateDifference(irSignal);


			

			difference = MovingAverage2(difference);


			

			double[] processedSignal =
				ApplyHammingWindow(difference);


			

			double threshold =
				CalculateThreshold(processedSignal);


			

			List<int> peaks =
				FindPeaks(
					processedSignal,
					threshold);


			

			int heartRate = -999;
			bool heartRateValid = false;

			if (peaks.Count >= 2)
			{
				double intervalSum = 0;

				for (int i = 1; i < peaks.Count; i++)
				{
					intervalSum +=
						peaks[i] - peaks[i - 1];
				}

				double averageInterval =
					intervalSum / (peaks.Count - 1);

				if (averageInterval > 0)
				{
					heartRate =
						(int)(6000.0 / averageInterval);

					heartRateValid = true;
				}
			}


		

			double spo2 = CalculateSpO2(
				irBuffer,
				redBuffer);

			bool spo2Valid =
				spo2 >= 0;


		

			return new PulseOximeterResult
			{
				HeartRate = heartRate,
				SpO2 = spo2,
				HeartRateValid = heartRateValid,
				SpO2Valid = spo2Valid
			};
		}


		

		private double[] MovingAverage4(
			double[] signal)
		{
			if (signal.Length < 4)
			{
				return signal;
			}

			double[] result =
				new double[signal.Length - 3];

			for (int i = 0;
				 i < result.Length;
				 i++)
			{
				result[i] =
					(
						signal[i] +
						signal[i + 1] +
						signal[i + 2] +
						signal[i + 3]
					) / 4.0;
			}

			return result;
		}


		

		private double[] CalculateDifference(
			double[] signal)
		{
			if (signal.Length < 2)
			{
				return signal;
			}

			double[] result =
				new double[signal.Length - 1];

			for (int i = 0;
				 i < signal.Length - 1;
				 i++)
			{
				result[i] =
					signal[i + 1] -
					signal[i];
			}

			return result;
		}


		

		private double[] MovingAverage2(
			double[] signal)
		{
			if (signal.Length < 2)
			{
				return signal;
			}

			double[] result =
				new double[signal.Length - 1];

			for (int i = 0;
				 i < signal.Length - 1;
				 i++)
			{
				result[i] =
					(
						signal[i] +
						signal[i + 1]
					) / 2.0;
			}

			return result;
		}


	

		private double[] ApplyHammingWindow(
			double[] signal)
		{
			int windowSize = 5;

			if (signal.Length < windowSize)
			{
				return signal;
			}

			double[] hamming =
			{
				0.08,
				0.54,
				1.00,
				0.54,
				0.08
			};

			double[] result =
				new double[
					signal.Length -
					windowSize + 1
				];

			for (int i = 0;
				 i < result.Length;
				 i++)
			{
				double sum = 0;

				for (int j = 0;
					 j < windowSize;
					 j++)
				{
					
					sum -=
						signal[i + j] *
						hamming[j];
				}

				result[i] = sum;
			}

			return result;
		}


		

		private double CalculateThreshold(
			double[] signal)
		{
			if (signal.Length == 0)
			{
				return 0;
			}

			double sum = 0;

			for (int i = 0;
				 i < signal.Length;
				 i++)
			{
				sum +=
					Math.Abs(signal[i]);
			}

			return sum /
				   signal.Length;
		}


		

		private List<int> FindPeaks(
			double[] signal,
			double threshold,
			int minDistance = 8,
			int maxPeaks = 15)
		{
			List<int> peaks =
				new List<int>();

			for (int i = 1;
				 i < signal.Length - 1;
				 i++)
			{
				bool isPeak =
					signal[i] > threshold &&
					signal[i] >
					signal[i - 1] &&
					signal[i] >=
					signal[i + 1];

				if (!isPeak)
				{
					continue;
				}

				if (peaks.Count == 0)
				{
					peaks.Add(i);
				}
				else
				{
					int previousPeak =
						peaks[^1];

					if (i - previousPeak >=
						minDistance)
					{
						peaks.Add(i);
					}
					else if (
						signal[i] >
						signal[previousPeak])
					{
						peaks[^1] = i;
					}
				}

				if (peaks.Count >=
					maxPeaks)
				{
					break;
				}
			}

			return peaks;
		}


		

		private double CalculateSpO2(
			int[] irBuffer,
			int[] redBuffer)
		{
			if (irBuffer.Length == 0 ||
				redBuffer.Length == 0)
			{
				return -999;
			}



			double irDc =
				irBuffer.Average();

			double redDc =
				redBuffer.Average();


			if (irDc == 0 ||
				redDc == 0)
			{
				return -999;
			}


			

			double irAc =
				CalculateStandardDeviation(
					irBuffer,
					irDc);

			double redAc =
				CalculateStandardDeviation(
					redBuffer,
					redDc);


			if (irAc == 0)
			{
				return -999;
			}


			// Ratio of Ratios

			double ratio =
				(redAc / redDc) /
				(irAc / irDc);


			

			double spo2 =
				-45.060 *
				ratio *
				ratio
				+
				30.354 *
				ratio
				+
				94.845;


			

			if (spo2 < 0 ||
				spo2 > 100)
			{
				return -999;
			}

			return Math.Round(
				spo2,
				1);
		}


		

		private double CalculateStandardDeviation(
			int[] values,
			double mean)
		{
			double sum = 0;

			for (int i = 0;
				 i < values.Length;
				 i++)
			{
				double difference =
					values[i] - mean;

				sum +=
					difference *
					difference;
			}

			double variance =
				sum /
				values.Length;

			return Math.Sqrt(
				variance);
		}


	

		private PulseOximeterResult InvalidResult()
		{
			return new PulseOximeterResult
			{
				HeartRate = -999,
				SpO2 = -999,
				HeartRateValid = false,
				SpO2Valid = false
			};
		}
	}
}