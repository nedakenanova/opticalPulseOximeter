namespace opticalPulse.Models
{
	public class PulseOximeterResult
	{
		public int HeartRate {  get; set; }
		public double SpO2 { get; set; }
		public bool HeartRateValid { get; set; }
		public bool SpO2Valid { get; set; }
	}
}
