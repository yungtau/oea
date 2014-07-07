﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建日期：20140705
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20140705 10:44
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rafy.ComponentModel;

namespace Rafy.UnitTest
{
    public abstract class EnvTest
    {
        public virtual void EnvTest_IOC_Interface()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterType<IAnimal, Dog>();

            var instance = container.Resolve<IAnimal>();
            Assert.IsTrue(instance is Dog);

            var cat = new Cat();
            container.RegisterInstance<IAnimal>(cat);
            instance = container.Resolve<IAnimal>();
            Assert.IsTrue(instance == cat);
        }

        public virtual void EnvTest_IOC_Interface_RegisterByType()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterInstance(typeof(IPlant), typeof(Tree));

            var instance = container.Resolve<IPlant>();
            Assert.IsTrue(instance is Tree);
            var instance2 = container.Resolve<IPlant>();
            Assert.IsTrue(instance == instance2);
        }

        /// <summary>
        /// Rabbit 依赖 IPlant，但是一样可以获取到它之后注册的 Tree 实例。
        /// </summary>
        public virtual void EnvTest_IOC_Interface_RegisterByType_Dependency()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterInstance(typeof(IAnimal), typeof(Rabbit));
            container.RegisterInstance(typeof(IPlant), typeof(Tree));

            var instance = container.Resolve<IAnimal>();
            Assert.IsTrue(instance is Rabbit);

            var rabbit =  instance as Rabbit;
            Assert.IsTrue(rabbit.Food is Tree);
        }

        /// <summary>
        /// 实例依赖的接口还没有注册的话，是无法创建实例的。
        /// </summary>
        public virtual void EnvTest_IOC_Interface_RegisterByType_Dependency_Error()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterInstance(typeof(IAnimal), typeof(Rabbit));
            bool success = false;
            try
            {
                var rabbit = container.Resolve<IAnimal>();
                success = true;
            }
            catch { }
            Assert.IsFalse(success);
        }

        /// <summary>
        /// 没有注册的接口是无法获取实例的。
        /// </summary>
        public virtual void EnvTest_IOC_Interface_ErrorWithoutRegister()
        {
            var container = ObjectContainerFactory.CreateContainer();
            bool success = true;
            try
            {
                container.Resolve<IAnimal>();
            }
            catch
            {
                success = false;
            }
            Assert.IsFalse(success);
        }

        public virtual void EnvTest_IOC_TypeByType()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterType<Dog, BigDog>();

            var instance = container.Resolve<Dog>();
            Assert.IsTrue(instance is BigDog);

            var smallDog = new SmallDog(instance as BigDog);
            container.RegisterInstance<Dog>(smallDog);
            instance = container.Resolve<Dog>();
            Assert.IsTrue(instance == smallDog);
        }

        public virtual void EnvTest_IOC_TypeBySelf()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterType<Dog, Dog>();

            var instance = container.Resolve<Dog>();
            Assert.AreEqual(instance.GetType(), typeof(Dog));
        }

        public virtual void EnvTest_IOC_TypeWithoutRegister()
        {
            var container = ObjectContainerFactory.CreateContainer();
            var dog = container.Resolve<Dog>();
            Assert.AreEqual(dog.GetType(), typeof(Dog));
            var bigDog = container.Resolve<BigDog>();
            Assert.AreEqual(bigDog.GetType(), typeof(BigDog));
        }

        public virtual void EnvTest_IOC_CtorNeedInterface()
        {
            var container = ObjectContainerFactory.CreateContainer();
            var smallDog = container.Resolve<SmallDog>();
            Assert.IsNotNull(smallDog.Parent);

            var bigDog = new BigDog();
            container.RegisterInstance(bigDog);
            var smallDog2 = container.Resolve<SmallDog>();
            Assert.AreEqual(bigDog, smallDog2.Parent);
        }

        public virtual void EnvTest_IOC_ResolveAll()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterType<IAnimal, Dog>();
            container.RegisterType<IAnimal, BigDog>("BigDog");
            container.RegisterType<IAnimal, SmallDog>("SmallDog");
            container.RegisterInstance<IAnimal>(new Cat { Name = "null" });
            container.RegisterInstance<IAnimal>(new Cat { Name = "another" }, "another");

            var instances = container.ResolveAll<IAnimal>();
            Assert.AreEqual(instances.Count(), 3);
            Assert.IsTrue(instances.Any(i => i is BigDog));
            Assert.IsTrue(instances.Any(i => i is SmallDog));
            Assert.IsTrue(instances.Any(i => i is Cat));
            Assert.IsTrue(instances.Any(i =>
            {
                var smallDog = i as SmallDog;
                if (smallDog != null)
                {
                    return smallDog.Parent != null;
                }
                return true;
            }));
            Assert.IsTrue(instances.Any(i =>
            {
                var cat = i as Cat;
                if (cat != null)
                {
                    return cat.Name == "another";
                }
                return true;
            }));
        }

        /// <summary>
        /// 后注册的项，可以覆盖前面注册的项。
        /// </summary>
        public virtual void EnvTest_IOC_Register_ReplaceByAfter()
        {
            var container = ObjectContainerFactory.CreateContainer();
            container.RegisterType<Dog, Dog>();
            container.RegisterType<Dog, BigDog>();
            container.RegisterType<Dog, SmallDog>();

            var instance = container.Resolve<Dog>();
            Assert.AreEqual(instance.GetType(), typeof(SmallDog));
        }

        #region Classes

        interface IAnimal { }

        interface IPlant { }

        class Tree : IPlant { }

        class Cat : IAnimal
        {
            public string Name { get; set; }
        }

        class Dog : IAnimal { }

        class BigDog : Dog { }

        class SmallDog : Dog
        {
            public BigDog Parent { get; set; }

            public SmallDog(BigDog parent)
            {
                this.Parent = parent;
            }
        }

        class Rabbit : IAnimal
        {
            public IPlant Food { get; set; }

            public Rabbit(IPlant food)
            {
                this.Food = food;
            }
        }

        #endregion
    }
}
