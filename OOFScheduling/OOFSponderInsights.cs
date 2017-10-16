﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace OOFScheduling
{
    internal static class OOFSponderInsights
    {

        private const string InfoEvent = "INFO";
        private const string AppName = "OOFSponer";

        //prep for ApplicationInsights right away so we can even instrument startup
        private static TelemetryClient appInsightsClient;

        public static void Track(string eventName)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            Track(eventName, properties);
        }

        public static void TrackInfo(string Details)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("Details", Details);
            Track(InfoEvent, properties);
        }

        public static void Track(string eventName, Dictionary<string, string> properties)
        {
            properties.Add("Application", AppName);
            AIClient.TrackEvent(eventName, properties);
        }

        public static void TrackException(String message, Exception exception)
        {
            Dictionary<string, string> _properties = new Dictionary<string, string>();
            _properties.Add("Application", AppName);

            Exception _exception = new Exception(message + ": " + exception.Message, exception);

            AIClient.TrackException(_exception, _properties);
        }

        public static TelemetryClient AIClient
        {
            get
            {
                if (appInsightsClient == null)
                {
                    appInsightsClient = new TelemetryClient();

                    //go set the application key, etc.
                    ConfigureApplicationInsights();
                }
                return appInsightsClient;
            }
        }

        public static bool ConfigureApplicationInsights()
        {
            bool isConfigured = false;

            try
            {
                AIClient.InstrumentationKey = "9eacd004 - 7944 - 4d2e-a978 - d66104c67a49";
                // Set session data:
                AIClient.Context.User.Id = Environment.UserName;

                //use DEBUGAI if we actually want AppInsights from a DEBUG build
#if DEBUGAI
            AIClient.Context.User.Id = "DEBUG";
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif

                AIClient.Context.Properties["MachineName"] = Environment.MachineName;

                isConfigured = true;
                OOFSponder.Logger.Info("Successfully configured ApplicationInsights");
            }
            catch (Exception ex)
            {
                OOFSponder.Logger.Error("Unable to configure ApplicationInsights: " + ex.Message);
            }

            return isConfigured;

        }
    }
}