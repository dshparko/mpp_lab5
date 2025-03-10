﻿using System.Collections.Generic;
using System.Linq;
using DependencyInjectionContainerLib;
using NUnit.Framework;

namespace Tests
 {
    [TestFixture]
    public class Tests
    {
        [Test]
        public void ValidationTest()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl>();
            dependencies.Register<IRepository, RepositoryImpl>();

            var provider = new DependencyProvider(dependencies);
            Assert.True(true);
        }


        [Test]
        public void AsSelfDependencyTest()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<RepositoryImpl, RepositoryImpl>();

            var provider = new DependencyProvider(dependencies);
            RepositoryImpl repository = provider.Resolve<RepositoryImpl>();
            Assert.NotNull(repository);
            Assert.IsInstanceOf(typeof(RepositoryImpl),repository);
        }

        [Test]
        public void RecursiveDependencyInjectionTest()
        {
            // конфигурация и использование контейнера
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl>();
            dependencies.Register<IRepository, RepositoryImpl>();

            var provider = new DependencyProvider(dependencies);
            
            var service1 = provider.Resolve<IService>();

            Assert.NotNull(service1);
        }

        [Test]
        public void RecursiveDependencyInjectionAsSelfTest()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<ServiceImpl, ServiceImpl>();
            dependencies.Register<IRepository, RepositoryImpl>();

            var provider = new DependencyProvider(dependencies);
            var serviceImpl = provider.Resolve<ServiceImpl>();

            Assert.NotNull(serviceImpl);
        }

        [Test]
        public void SingletonTest()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl>(LifeCycle.Singleton);
            dependencies.Register<IRepository, RepositoryImpl>(LifeCycle.Singleton);

            var provider = new DependencyProvider(dependencies);
            
            var service1 = provider.Resolve<IService>();
            var service2 = provider.Resolve<IService>();

            Assert.AreEqual(service1, service2);
        }

        [Test]
        public void InstancePerDependencyTest()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl>(LifeCycle.InstancePerDependency);
            dependencies.Register<IRepository, RepositoryImpl>(LifeCycle.Singleton);

            var provider = new DependencyProvider(dependencies);

            var service1 = provider.Resolve<IService>();
            var service2 = provider.Resolve<IService>();

            Assert.AreNotEqual(service1, service2);
        }

        
        [Test]
        public void GenericConfigurationTest()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, MySqlRepository>(LifeCycle.Singleton);
            dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();

            var provider = new DependencyProvider(dependencies);
            Assert.True(true);
        }
        
        
        [Test]
        public void GetGenericClassTest()
        {
            var dependencies = new DependenciesConfiguration();
           
            dependencies.Register<IRepository, MySqlRepository>();
            dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();
            var provider = new DependencyProvider(dependencies);
            
            var service = provider.Resolve<IService<IRepository>>();
            
            Assert.NotNull(service);
            Assert.IsInstanceOf(typeof(ServiceImpl<IRepository>),service);
        }
        
       
        // А-Б-А
        [Test]
        public void ABA_test()
        {   
            var config = new DependenciesConfiguration();
            config.Register<IA, ClassA>(LifeCycle.Singleton);
            config.Register<IB, ClassB>(LifeCycle.Singleton);
            DependencyProvider provider = new DependencyProvider(config);
            ClassB b = (ClassB)provider.Resolve<IB>();
            ClassA a = (ClassA)provider.Resolve<IA>();
            Assert.AreEqual(b.ia, a);
            Assert.AreEqual(b, a.ib);
            ClassA a1 = (ClassA)b.ia;
            Assert.AreEqual(a1.ib, b);
        }
        
        [Test]
        public void QWEQ_test()
        {
            var dependencies = new DependenciesConfiguration();
            var provider = new DependencyProvider(dependencies);
            dependencies.Register<IQ, Q>(LifeCycle.Singleton);
            dependencies.Register<IW, W>(LifeCycle.Singleton);
            dependencies.Register<IE, E>(LifeCycle.Singleton);
            Q q = (Q)provider.Resolve<IQ>();
            W w = (W)provider.Resolve<IW>();
            E e = (E)provider.Resolve<IE>();
            Assert.AreSame(q,e.iq);
            Assert.AreSame(w.ie,e);
            Assert.AreSame(w.iq,q);
        }
    }
}