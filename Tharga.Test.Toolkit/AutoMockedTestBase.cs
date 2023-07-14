//using System;
//using System.Linq.Expressions;
//using Moq;
//using Moq.AutoMock;

//namespace Tharga.Test.Toolkit
//{
//    public abstract class AutoMockedTestBase : IDisposable
//    {
//        private readonly AutoMocker _mocker;

//        private object _sut;

//        protected AutoMockedTestBase()
//        {
//            _mocker = new AutoMocker();
//        }

//        public void Dispose()
//        {
//            _sut = null;
//        }

//        /// <summary>
//        ///     Gives access to the subject under specification.
//        ///     On first access the spec tries to create an instance of the subject type by itself.
//        /// </summary>
//        /// <typeparam name="T">The type of subject to create.</typeparam>
//        /// <returns>The subject of the specification.</returns>
//        protected T SystemUnderTest<T>() where T : class
//        {
//            if (_sut != null) return _sut as T;

//            _sut = _mocker.CreateInstance<T>();

//            return (T)_sut;
//        }

//        /// <summary>
//        ///     Creates a fake of the type specified by <typeparamref name="TDependency" />.
//        ///     This method reuses existing instances. If an instance of <typeparamref name="TDependency" />
//        ///     was already requested it's returned here. (You can say this is kind of a singleton behavior)
//        ///     Besides that, you can obtain a reference to injected instances/fakes with this method.
//        /// </summary>
//        /// <typeparam name="TDependency">The type to create a fake for. (Should be an interface or an abstract class)</typeparam>
//        /// <returns>
//        ///     An instance implementing <typeparamref name="TDependency" />.
//        /// </returns>
//        protected Mock<TDependency> The<TDependency>() where TDependency : class
//        {
//            return _mocker.GetMock<TDependency>();
//        }

//        /// <summary>
//        ///     Configures the specification to use the specified instance for <typeparamref name="TDependency" />.
//        /// </summary>
//        /// <typeparam name="TDependency">The type to inject.</typeparam>
//        /// <param name="instance">The instance to inject.</param>
//        protected void With<TDependency>(TDependency instance)
//        {
//            _mocker.Use(instance);
//        }

//        /// <summary>
//        ///     Configures the specification to use the specified mock for <typeparamref name="TDependency" />.
//        /// </summary>
//        /// <typeparam name="TDependency">The type to inject.</typeparam>
//        /// <param name="mock">The mock to inject.</param>
//        protected void With<TDependency>(Mock<TDependency> mock) where TDependency : class
//        {
//            _mocker.Use(mock);
//        }

//        /// <summary>
//        ///     Configures the specification to setup a mock with specified behavior for <typeparamref name="TDependency" />.
//        /// </summary>
//        /// <typeparam name="TDependency">The type to inject.</typeparam>
//        /// <param name="setup">The behavior to inject.</param>
//        protected void With<TDependency>(Expression<Func<TDependency, bool>> setup) where TDependency : class
//        {
//            _mocker.Use(setup);
//        }
//    }
//}