using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnvironmentMonitoringMicroservice
{
    // Class representing the environment data collected by the monitoring station
    public class EnvironmentData
    {
        public ObjectId Id { get; set; } // Unique identifier for the data
        public double Temperature { get; set; } // Temperature data
        public double Rainfall { get; set; } // Rainfall data
        public int Humidity { get; set; } // Humidity data
        public int AirPollution { get; set; } // Air pollution data
        public double CO2Emissions { get; set; } // CO2 emissions data
        public DateTime Timestamp { get; set; } // Timestamp indicating when the data was collected
    }

    // Class representing the Monitoring Station API responsible for collecting and storing environment data
    public class MonitoringStationAPI
    {
        private readonly SensorsAPI _sensorsAPI; // Instance of the SensorsAPI for retrieving sensor data
        private readonly IMongoCollection<EnvironmentData> _environmentDataCollection; // MongoDB collection for storing environment data

        // Constructor
        public MonitoringStationAPI()
        {
            _sensorsAPI = new SensorsAPI();

            // Connect to MongoDB server
            var client = new MongoClient("mongodb://localhost:27017");

            // Access database
            var database = client.GetDatabase("EnvironmentMonitoringDB");

            // Access collection
            _environmentDataCollection = database.GetCollection<EnvironmentData>("EnvironmentData");
        }

        // Method to collect and store data continuously
        public async Task CollectAndStoreData()
        {
            while (true)
            {
                // Retrieve data from sensors
                var temperature = await _sensorsAPI.GetTemperatureData();
                var rainfall = await _sensorsAPI.GetRainfallData();
                var humidity = await _sensorsAPI.GetHumidityData();
                var airPollution = await _sensorsAPI.GetAirPollutionData();
                var co2Emissions = await _sensorsAPI.GetCO2EmissionsData();

                // Check thresholds and generate alerts if necessary
                CheckThresholdsAndGenerateAlerts(temperature, rainfall, humidity, airPollution, co2Emissions);

                // Create a document to store in MongoDB
                var document = new EnvironmentData
                {
                    Temperature = temperature,
                    Rainfall = rainfall,
                    Humidity = humidity,
                    AirPollution = airPollution,
                    CO2Emissions = co2Emissions,
                    Timestamp = DateTime.UtcNow
                };

                // Insert document into MongoDB collection
                await _environmentDataCollection.InsertOneAsync(document);

                // Delay for specified intervals before collecting data again
                await Task.Delay(TimeSpan.FromSeconds(30)); // Temperature and Rainfall
                // await Task.Delay(TimeSpan.FromMinutes(1)); // Humidity and Air Pollution
                // await Task.Delay(TimeSpan.FromMinutes(2)); // CO2 Emissions
            }
        }
        private void CheckThresholdsAndGenerateAlerts(double temperature, double rainfall, int humidity, int airPollution, double co2Emissions)
        {
            // Check temperature threshold
            if (temperature < -9 || temperature > 30)
            {
                Console.WriteLine("Temperature threshold crossed! Alert generated.");
            }

            // Check rainfall threshold
            if (rainfall > 32)
            {
                Console.WriteLine("Rainfall threshold crossed! Alert generated.");
            }

            // Check humidity threshold
            if (humidity < 0 || humidity > 100)
            {
                Console.WriteLine("Humidity threshold crossed! Alert generated.");
            }

            // Check air pollution threshold
            if (airPollution < 1 || airPollution > 9)
            {
                Console.WriteLine("Air pollution threshold crossed! Alert generated.");
            }

            // Check CO2 emissions threshold
            if (co2Emissions < 1 || co2Emissions > 100)
            {
                Console.WriteLine("CO2 emissions threshold crossed! Alert generated.");
            }
        }
        
        // Method to retrieve environment data
        public async Task<EnvironmentData> GetEnvironmentData()
        {
            // Implement method to retrieve environment data
            return await _environmentDataCollection.Find(_ => true).FirstOrDefaultAsync();
        }

        // Methods to retrieve latest data for each sensor
        public async Task<double> GetLatestTemperatureData()
        {
            return await _sensorsAPI.GetTemperatureData();
        }

        public async Task<double> GetLatestRainfallData()
        {
            return await _sensorsAPI.GetRainfallData();
        }

        public async Task<int> GetLatestHumidityData()
        {
            return await _sensorsAPI.GetHumidityData();
        }

        public async Task<int> GetLatestAirPollutionData()
        {
            return await _sensorsAPI.GetAirPollutionData();
        }

        public async Task<double> GetLatestCO2EmissionsData()
        {
            return await _sensorsAPI.GetCO2EmissionsData();
        }
    }

    // Class representing the Sensors API responsible for generating sensor data
    public class SensorsAPI
    {
        private readonly Random _random = new Random();
        private Dictionary<string, DateTime> lastThresholdViolationTimes = new Dictionary<string, DateTime>();

        // Methods to generate sensor data
        public async Task<double> GetTemperatureData()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            return GenerateTemperatureData();
        }

        public async Task<double> GetRainfallData()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            return GenerateRainfallData();
        }

        public async Task<int> GetHumidityData()
        {
            await Task.Delay(TimeSpan.FromMinutes(1));
            return GenerateHumidityData();
        }

        public async Task<int> GetAirPollutionData()
        {
            await Task.Delay(TimeSpan.FromMinutes(1));
            return GenerateAirPollutionData();
        }

        public async Task<double> GetCO2EmissionsData()
        {
            await Task.Delay(TimeSpan.FromMinutes(2));
            return GenerateCO2EmissionsData();
        }

        // Methods to generate out-of-threshold data for each sensor

        private double GenerateTemperatureData()
        {
            if (ShouldGenerateThresholdData("Temperature"))
            {
                return _random.NextDouble() * (100 - (-100)) + (-100); // Generate out-of-threshold data
            }
            return _random.NextDouble() * (39 - (-20)) + (-20); // Generate normal data
        }

        private double GenerateRainfallData()
        {
            if (ShouldGenerateThresholdData("Rainfall"))
            {
                return _random.NextDouble() * 100; // Generate out-of-threshold data
            }
            return _random.NextDouble() * 40; // Generate normal data
        }

        private int GenerateHumidityData()
        {
            if (ShouldGenerateThresholdData("Humidity"))
            {
                return _random.Next(-50, 150); // Generate out-of-threshold data
            }
            return _random.Next(0, 101); // Generate normal data
        }

        private int GenerateAirPollutionData()
        {
            if (ShouldGenerateThresholdData("AirPollution"))
            {
                return _random.Next(-5, 15); // Generate out-of-threshold data
            }
            return _random.Next(1, 11); // Generate normal data
        }

        private double GenerateCO2EmissionsData()
        {
            if (ShouldGenerateThresholdData("CO2Emissions"))
            {
                return _random.NextDouble() * 200; // Generate out-of-threshold data
            }
            return _random.NextDouble() * 100; // Generate normal data
        }

        // Method to determine whether to generate out-of-threshold data based on the last violation time
        private bool ShouldGenerateThresholdData(string sensorType)
        {
            if (!lastThresholdViolationTimes.ContainsKey(sensorType) || DateTime.UtcNow - lastThresholdViolationTimes[sensorType] >= TimeSpan.FromMinutes(2))
            {
                lastThresholdViolationTimes[sensorType] = DateTime.UtcNow;
                return true;
            }
            return false;
        }
    }

    // Class for configuring the application services and request handling
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(); // Add controller services
            services.AddSingleton<MonitoringStationAPI>(); // Register MonitoringStationAPI as a singleton service
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Use developer exception page in development environment
            }
            else
            {
                app.UseExceptionHandler("/Error"); // Use error handler page for other environments
                app.UseHsts(); // Use HTTP Strict Transport Security (HSTS)
            }

            app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
            app.UseStaticFiles(); // Enable serving static files
            app.UseRouting(); // Enable routing
            app.UseAuthorization(); // Enable authorization

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Map controllers
            });

            // Start collecting and storing data
            var monitoringStationAPI = app.ApplicationServices.GetRequiredService<MonitoringStationAPI>();
            monitoringStationAPI.CollectAndStoreData();
        }
    } 

    // Controller class for handling environment data requests
    [ApiController]
    public class EnvironmentDataController : ControllerBase
    {
        private readonly MonitoringStationAPI _monitoringStationAPI;

        public EnvironmentDataController(MonitoringStationAPI monitoringStationAPI)
        {
            _monitoringStationAPI = monitoringStationAPI;
        }

        // Endpoint to get all environment data
        [HttpGet("/environment/data")]
        public async Task<ActionResult<EnvironmentData>> GetAllEnvironmentData()
        {
            var data = await _monitoringStationAPI.GetEnvironmentData();
            return Ok(data);
        }

        // Endpoint to get the latest temperature data
        [HttpGet("/sensors/temperature")]
        public async Task<ActionResult<double>> GetLatestTemperatureData()
        {
            var temperature = await _monitoringStationAPI.GetLatestTemperatureData();
            return Ok(temperature);
        }

        // Endpoint to get the latest rainfall data
        [HttpGet("/sensors/rainfall")]
        public async Task<ActionResult<double>> GetLatestRainfallData()
        {
            var rainfall = await _monitoringStationAPI.GetLatestRainfallData();
            return Ok(rainfall);
        }

        // Endpoint to get the latest humidity data
        [HttpGet("/sensors/humidity")]
        public async Task<ActionResult<int>> GetLatestHumidityData()
        {
            var humidity = await _monitoringStationAPI.GetLatestHumidityData();
            return Ok(humidity);
        }

        // Endpoint to get the latest air pollution data
        [HttpGet("/sensors/air-pollution")]
        public async Task<ActionResult<int>> GetLatestAirPollutionData()
        {
            var airPollution = await _monitoringStationAPI.GetLatestAirPollutionData();
            return Ok(airPollution);
        }

        // Endpoint to get the latest CO2 emissions data
        [HttpGet("/sensors/co2-emissions")]
        public async Task<ActionResult<double>> GetLatestCO2EmissionsData()
        {
            var co2Emissions = await _monitoringStationAPI.GetLatestCO2EmissionsData();
            return Ok(co2Emissions);
        }
    }

    // Entry point of the application
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>(); // Use Startup class to configure the application
                })
                .Build();

            await host.RunAsync(); // Run the application
        }
    }
}
