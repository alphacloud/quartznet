using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;

using Quartz.Impl;
using Quartz.Spi;

namespace Quartz
{
    public interface IConfigurationBuilder
    {
        /// <summary>
        ///     Builds configuration.
        /// </summary>
        /// <returns>Configuration settings.</returns>
        NameValueCollection Build();

        IConfigurationBuilder WithSchedulerConfiguration(Action<ISchedulerConfigurationBuilder> schedulerConfigurator);
    }

    public class ConfigurationBuilder : IConfigurationBuilder
    {
        private readonly NameValueCollection configuration = new NameValueCollection();

        private ConfigurationBuilder()
        {
        }

        public static ConfigurationBuilder Create()
        {
            return new ConfigurationBuilder();
        }

        /// <summary>
        ///     Builds configuration.
        /// </summary>
        /// <returns>Configuration settings.</returns>
        public NameValueCollection Build()
        {
            return configuration;
        }

        public IConfigurationBuilder WithSchedulerConfiguration(Action<ISchedulerConfigurationBuilder> schedulerConfigurator)
        {
            if (schedulerConfigurator == null)
            {
                throw new ArgumentNullException("schedulerConfigurator");
            }

            var scb = new SchedulerConfigurationBuilder(this);
            schedulerConfigurator(scb);

            return this;
        }

        public ISchedulerConfigurationBuilder WithSchedulerConfiguration()
        {
            return new SchedulerConfigurationBuilder(this);
        }

        internal void SetProperty(string propName, bool value)
        {
            configuration[propName] = value.ToString(CultureInfo.InvariantCulture);
        }

        internal void SetProperty(string propName, TimeSpan idleWaitTime)
        {
            configuration[propName] = ToMilliseconds(idleWaitTime);
        }

        internal void SetProperty(string propName, int value)
        {
            configuration[propName] = value.ToString(CultureInfo.InvariantCulture);
        }

        private static string ToMilliseconds(TimeSpan timeSpan)
        {
            return ((long) timeSpan.TotalMilliseconds).ToString(CultureInfo.InvariantCulture);
        }

        internal static string GetTypeName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        internal void SetProperty(string propName, string value)
        {
            configuration[propName] = value;
        }

        internal void SetType<T>(string propertyName) where T : class
        {
            configuration[propertyName] = GetTypeName(typeof (T));
        }
    }

    public interface ISchedulerConfigurationBuilder
    {
        ISchedulerConfigurationBuilder InstanceName(string instanceName);

        ISchedulerConfigurationBuilder InstanceId(string instanceId);

        ISchedulerConfigurationBuilder WithInstanceIdGenerator<TInstanceIdGenerator>(IEnumerable<KeyValuePair<string, string>> properties)
            where TInstanceIdGenerator : class, IInstanceIdGenerator;

        ISchedulerConfigurationBuilder ThreadName(string threadName);

        ISchedulerConfigurationBuilder BatchTimeWindow(TimeSpan window);

        ISchedulerConfigurationBuilder MaxBatchSize(int maxBatchSize);

        ISchedulerConfigurationBuilder WithExporter<TExporter>()
            where TExporter : class, ISchedulerExporter;

        ISchedulerConfigurationBuilder Proxy(bool useProxy);

        ISchedulerConfigurationBuilder WithRemoteProxyFactory<TProxyFactory>()
            where TProxyFactory : class, IRemotableSchedulerProxyFactory;

        ISchedulerConfigurationBuilder IdleWaitTime(TimeSpan idleWaitTime);

        ISchedulerConfigurationBuilder WithJobFactory<TJobFactory>()
            where TJobFactory : class, IJobFactory;

        ISchedulerConfigurationBuilder DbFailureInterval(TimeSpan interval);

        ISchedulerConfigurationBuilder MakeThreadDaemon(bool enable);
    }

    internal class SchedulerConfigurationBuilder : ISchedulerConfigurationBuilder
    {
        private readonly ConfigurationBuilder builder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        internal SchedulerConfigurationBuilder(ConfigurationBuilder builder)
        {
            this.builder = builder;
        }

        public ISchedulerConfigurationBuilder InstanceName(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                throw new ArgumentException("Instance name should be specified", "instanceName");
            }
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerInstanceName, instanceName);

            return this;
        }

        public ISchedulerConfigurationBuilder InstanceId(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                throw new ArgumentException("Instance Id should be specified");
            }
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerInstanceId, instanceId);

            return this;
        }

        public ISchedulerConfigurationBuilder WithInstanceIdGenerator<TInstanceIdGenerator>(IEnumerable<KeyValuePair<string, string>> properties)
            where TInstanceIdGenerator : class, IInstanceIdGenerator
        {
            builder.SetType<TInstanceIdGenerator>(StdSchedulerFactory.PropertySchedulerInstanceIdGeneratorType);
            // todo: import properties
            return this;
        }

        public ISchedulerConfigurationBuilder ThreadName(string threadName)
        {
            if (string.IsNullOrEmpty(threadName))
            {
                throw new ArgumentException("Thread name should be specified");
            }
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerThreadName, threadName);

            return this;
        }

        public ISchedulerConfigurationBuilder BatchTimeWindow(TimeSpan window)
        {
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerBatchTimeWindow, window);

            return this;
        }

        public ISchedulerConfigurationBuilder MaxBatchSize(int maxBatchSize)
        {
            if (maxBatchSize <= 0)
            {
                throw new ArgumentException("MaxBatchSize must be positive number");
            }

            builder.SetProperty(StdSchedulerFactory.PropertySchedulerMaxBatchSize, maxBatchSize);
            return this;
        }

        public ISchedulerConfigurationBuilder WithExporter<TExporter>()
            where TExporter : class, ISchedulerExporter
        {
            builder.SetType<TExporter>(StdSchedulerFactory.PropertySchedulerExporterType);
            // todo: import properties
            return this;
        }

        public ISchedulerConfigurationBuilder Proxy(bool useProxy)
        {
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerProxy, useProxy);
            return this;
        }

        public ISchedulerConfigurationBuilder WithRemoteProxyFactory<TProxyFactory>()
            where TProxyFactory : class, IRemotableSchedulerProxyFactory
        {
            builder.SetType<TProxyFactory>(StdSchedulerFactory.PropertySchedulerProxyType);
            // todo: import properties
            return this;
        }


        public ISchedulerConfigurationBuilder WithTypeLoadHelper<TTypeLoadHelper>() where TTypeLoadHelper : class, ITypeLoadHelper
        {
            builder.SetType<TTypeLoadHelper>(StdSchedulerFactory.PropertySchedulerTypeLoadHelperType);
            throw new NotImplementedException();
        }

        public ISchedulerConfigurationBuilder IdleWaitTime(TimeSpan idleWaitTime)
        {
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerIdleWaitTime, idleWaitTime);
            return this;
        }

        public ISchedulerConfigurationBuilder WithJobFactory<TJobFactory>()
            where TJobFactory : class, IJobFactory
        {
            builder.SetType<IJobFactory>(StdSchedulerFactory.PropertySchedulerJobFactoryType);
            // todo: import properties
            return this;
        }

        public ISchedulerConfigurationBuilder InterruptJobsOnShotdown(bool value)
        {
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerInterruptJobsOnShutdown, value);
            return this;
        }

        public ISchedulerConfigurationBuilder InterruptJobsOnShotdownWithWait(bool value)
        {
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerInterruptJobsOnShutdownWithWait, value);
            return this;
        }


        public ISchedulerConfigurationBuilder WithContext(IDictionary<string, object> contextItems)
        {
            if (contextItems == null)
            {
                throw new ArgumentNullException("contextItems");
            }

            // todo: add as quartz.context.key = value
            foreach (var key in contextItems.Keys)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("Empty context keys are not allowed", "contextItems");
                
                // todo: lowercase key name
                builder.SetProperty("quartz.context." + key, contextItems[key].ToString());
            }
            return this;
        }

        public ISchedulerConfigurationBuilder DbFailureInterval(TimeSpan interval)
        {
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerDbFailureRetryInterval, interval);
            return this;
        }

        public ISchedulerConfigurationBuilder MakeThreadDaemon(bool enable)
        {
            builder.SetProperty(StdSchedulerFactory.PropertySchedulerMakeSchedulerThreadDaemon, enable);

            return this;
        }
    }
}