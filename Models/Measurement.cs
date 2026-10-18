namespace opticalPulse.Models
{
	public class Measurement
	{
		public int Id {  get; set; }
		public int Pulse {  get; set; }
		public double SpO2 {  get; set; }
		public int RedValue {  get; set; }
		public int IrValue {  get; set; }
		public DateTime MeasureAt { get; set; }
		
	}
}
