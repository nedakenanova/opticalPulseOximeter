using Microsoft.AspNetCore.Mvc;
using opticalPulse.Models;
using opticalPulse.Services;

namespace opticalPulse.Controllers
{
	public class MeasurementsController : Controller
	{
		private readonly PulseOximeterService _pulseOximeterService;

		public MeasurementsController()
		{
			_pulseOximeterService = new PulseOximeterService();
		}

		public IActionResult Index()
		{
			
			int[] irBuffer =
			{
				50000, 50500, 51200, 52000, 53000,
				54000, 55000, 54500, 53500, 52500,
				51500, 50700, 50200, 50000, 50300,
				51000, 52000, 53200, 54400, 55200,
				54800, 53800, 52600, 51600, 50800,
				50200, 50000, 50500, 51300, 52300,
				53500, 54600, 55300, 54700, 53600,
				52400, 51400, 50600, 50100, 50000
			};

			
			int[] redBuffer =
			{
				48000, 48400, 49000, 49700, 50500,
				51300, 52000, 51600, 50800, 50000,
				49300, 48700, 48200, 48000, 48300,
				48900, 49700, 50600, 51500, 52100,
				51700, 50900, 50000, 49300, 48600,
				48200, 48000, 48400, 49100, 49900,
				50800, 51600, 52200, 51700, 50800,
				49900, 49100, 48500, 48100, 48000
			};

			PulseOximeterResult result =
				_pulseOximeterService.Calculate(
					irBuffer,
					redBuffer);

			return View(result);
		}
	}
}