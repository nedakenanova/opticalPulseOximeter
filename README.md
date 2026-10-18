# Optical Pulse Oximeter

ASP.NET Core MVC application for processing and visualizing pulse rate and oxygen saturation (SpO₂) data from a MAX30102-based pulse oximeter system.

## Project overview

The project was developed as a university course project for the design of an optical pulse oximeter. The proposed system combines:

- MAX30102 optical pulse-oximeter sensor
- Arduino Uno / ATmega328P
- I²C communication between the sensor and the controller
- USB/Serial communication with a computer
- ASP.NET Core MVC application for signal processing and visualization

The software part is designed to process RED and IR photoplethysmographic data and calculate heart rate and SpO₂.

## Algorithm

The signal-processing logic is based on the MAXREFDES117 reference design by Maxim Integrated (now Analog Devices).

The processing stages include:

1. Removal of the DC component from the IR signal
2. Moving-average smoothing
3. Signal differentiation
4. Additional smoothing and Hamming-window processing
5. Detection of characteristic PPG peaks
6. Heart-rate calculation
7. RED/IR AC and DC component analysis
8. Ratio-of-Ratios calculation
9. SpO₂ estimation
10. Validation of the calculated results

The original reference algorithm is written in C/C++. The project adapts its main processing logic to C#.

## Application architecture

The application uses the ASP.NET Core MVC architecture.

- **Models** – sensor samples, measurements and calculated results
- **Services** – PPG signal-processing and pulse-oximetry calculations
- **Controllers** – application flow and communication between services and views
- **Views** – visualization of heart rate and SpO₂ results

Main data flow:

`RED / IR data → PulseOximeterService → PulseOximeterResult → MeasurementsController → View`

## Technologies

- C#
- .NET 8
- ASP.NET Core MVC
- HTML / CSS
- MAX30102
- Arduino Uno / ATmega328P
- I²C
- USB / Serial

## Current implementation

At the current stage, the ASP.NET Core application can work with sample RED and IR input arrays in order to demonstrate the processing algorithm and the application flow.

A future hardware implementation can replace the sample data with measurements received from Arduino Uno through USB/Serial communication.

## Running the project

Requirements:

- .NET 8 SDK
- Visual Studio 2022 or another compatible .NET development environment

Clone the repository:

```bash
git clone https://github.com/nedakenanova/opticalPulseOximeter.git
```

Open the project and run it from Visual Studio, or use:

```bash
dotnet run
```

## Planned improvements

- Real-time data acquisition from Arduino Uno
- Live RED and IR PPG graphs
- Measurement history
- Improved validation of signal quality
- Closer one-to-one C# port of the original MAXREFDES117 SpO₂ lookup-table implementation

## Reference

Analog Devices / Maxim Integrated, **MAXREFDES117: Heart-Rate and Pulse-Oximetry Monitor**.

---

This project is intended for educational and demonstration purposes and is not a certified medical device.
