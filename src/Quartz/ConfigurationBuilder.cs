using System;
using System.Collections.Specialized;
using System.Globalization;

using Quartz.Impl;
using Quartz.Spi;

namespace Quartz
{
    public class ConfigurationBuilder
    {
        private readonly NameValueCollection configurationValues = new NameValueCollection();

        private ConfigurationBuilder()
        {
        }

        public static ConfigurationBuilder Create()
        {
            return new ConfigurationBuilder();
        }

        public ConfigurationBuilder WithInstanceName(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                throw new ArgumentException("Instance name should be specified", "instanceName");
            }
            configurationValues[StdSchedulerFactory.PropertySchedulerInstanceName] = instanceName;

            return this;
        }

        public ConfigurationBuilder WithInstanceId(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                throw new ArgumentException("Instance Id should be specified");
            }
            configurationValues[StdSchedulerFactory.PropertySchedulerInstanceId] = instanceId;

            return this;
        }

        public ConfigurationBuilder WithInstanceIdGeneratorPreperties(NameValueCollection properties)
        {
            throw new NotImplementedException();
        }

        public ConfigurationBuilder WithInstanceIdGenerator<TInstanceIdGenerator>()
            where TInstanceIdGenerator : IInstanceIdGenerator
        {
            configurationValues[StdSchedulerFactory.PropertySchedulerInstanceIdGeneratorType] = GetTypeName(typeof (TInstanceIdGenerator));
            // todo: import properties
            return this;
        }

        public ConfigurationBuilder WithThreadName(string threadName)
        {
            if (string.IsNullOrEmpty(threadName))
            {
                throw new ArgumentException("Thread name should be specified");
            }
            configurationValues[StdSchedulerFactory.PropertySchedulerThreadName] = threadName;

            return this;
        }

        public ConfigurationBuilder WithBatchTimeWindow(TimeSpan window)
        {
            configurationValues[StdSchedulerFactory.PropertySchedulerBatchTimeWindow] = ((long) window.TotalMilliseconds).ToString(CultureInfo.InvariantCulture);

            return this;
        }

        public ConfigurationBuilder WithMazBatchSize(int maxBatchSize)
        {
            if (maxBatchSize <= 0)
            {
                throw new ArgumentException("MaxBatchSize must be positive number");
            }

            configurationValues[StdSchedulerFactory.PropertySchedulerMaxBatchSize] = maxBatchSize.ToString(CultureInfo.InvariantCulture);
            return this;
        }

        public ConfigurationBuilder WithExporter<TExporter>()
            where TExporter : ISchedulerExporter
        {
            configurationValues[StdSchedulerFactory.PropertySchedulerExporterType] = GetTypeName(typeof (TExporter));
            // todo: import properties
            return this;
        }

        public ConfigurationBuilder UseProxy(bool proxy)
        {
            configurationValues[StdSchedulerFactory.PropertySchedulerProxy] = proxy.ToString(CultureInfo.InvariantCulture);
            return this;
        }

        public ConfigurationBuilder WithRemoteProxyFactory<TProxyFactory>()
            where TProxyFactory : IRemotableSchedulerProxyFactory
        {
            configurationValues[StdSchedulerFactory.PropertySchedulerProxyType] = GetTypeName(typeof (TProxyFactory));
            // todo: import properties
            return this;
        }

        public ConfigurationBuilder WithIdleWaitTime(TimeSpan idleWaitTime)
        {
            SetTimeSpan(StdSchedulerFactory.PropertySchedulerIdleWaitTime, idleWaitTime);
            return this;
        }

        public ConfigurationBuilder WithJobFactory<TJobFactory>()
            where TJobFactory : IJobFactory
        {
            configurationValues[StdSchedulerFactory.PropertySchedulerJobFactoryType] = GetTypeName(typeof (TJobFactory));
            // todo: import properties
            return this;
        }

        public ConfigurationBuilder WithDbFailureInterval(TimeSpan interval)
        {
            SetTimeSpan(StdSchedulerFactory.PropertySchedulerDbFailureRetryInterval, interval);
            return this;
        }

        public ConfigurationBuilder MakeThreadDaemon(bool enable)
        {
            SetBoolean(StdSchedulerFactory.PropertySchedulerMakeSchedulerThreadDaemon, enable);

            return this;
        }

        /// <summary>
        ///     Builds configuration.
        /// </summary>
        /// <returns>Configuration settings.</returns>
        public NameValueCollection Build()
        {
            return configurationValues;
        }

        private void SetBoolean(string propName, bool value)
        {
            configurationValues[propName] = value.ToString(CultureInfo.InvariantCulture);
        }

        private void SetTimeSpan(string propName, TimeSpan idleWaitTime)
        {
            configurationValues[propName] = ToMilliseconds(idleWaitTime);
        }

        private static string ToMilliseconds(TimeSpan timeSpan)
        {
            return ((long) timeSpan.TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
        }

        private static string GetTypeName(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}