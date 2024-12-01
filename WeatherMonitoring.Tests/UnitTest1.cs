using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EnvironmentMonitoringMicroservice;

namespace WeatherMonitoring.Tests;
namespace EnvironmentMonitoringMicroservice.Tests
{
    [TestClass]
    public class MonitoringStationAPITests
    {
        [TestMethod]
        public void CheckThresholdsAndGenerateAlerts_TemperatureBelowThreshold_AlertGenerated()
        {
            // Arrange
            var monitoringStationAPI = new MonitoringStationAPI();
            double temperature = -10; // Below threshold value

            // Act
            monitoringStationAPI.CheckThresholdsAndGenerateAlerts(temperature, 20, 50, 5, 30);

            // Assert
            // Add assertions here
        }

        [TestMethod]
        public void CheckThresholdsAndGenerateAlerts_TemperatureAboveThreshold_AlertGenerated()
        {
            // Arrange
            var monitoringStationAPI = new MonitoringStationAPI();
            double temperature = 40; // Above threshold value

            // Act
            monitoringStationAPI.CheckThresholdsAndGenerateAlerts(temperature, 20, 50, 5, 30);

            // Assert
            // Add assertions here
        }

        // Add more test methods for other scenarios
    }

    [TestClass]
    public class SensorsAPITests
    {
        [TestMethod]
        public void GetTemperatureData_ReturnsValidTemperature()
        {
            // Arrange
            var sensorsAPI = new SensorsAPI();

            // Act
            var temperature = sensorsAPI.GetTemperatureData().Result;

            // Assert
            // Add assertions here
        }

        [TestMethod]
        public void GetRainfallData_ReturnsValidRainfall()
        {
            // Arrange
            var sensorsAPI = new SensorsAPI();

            // Act
            var rainfall = sensorsAPI.GetRainfallData().Result;

            // Assert
            // Add assertions here
        }

        // Add more test methods for other sensors

        [TestMethod]
        public void CheckThresholdsAndGenerateAlerts_TemperatureBelowThreshold_AlertGenerated()
        {
            // Arrange
            var sensorsAPI = new SensorsAPI();

            // Act
            var temperature = sensorsAPI.GetTemperatureData().Result;

            // Assert
            Assert.IsTrue(temperature < -9 || temperature > 30, "Temperature out of range alert not generated.");
        }

        // Add similar tests for other sensors

        [TestMethod]
        public void CollectAndStoreData_ServerPushTest_DataStoredInDatabase()
        {
            // Arrange
            var monitoringStationAPI = new MonitoringStationAPI();

            // Act
            monitoringStationAPI.CollectAndStoreData().Wait();

            // Assert
            // Add assertions here to check if data is stored in the database
        }
    }

    // Add more test classes for other components if necessary
}
