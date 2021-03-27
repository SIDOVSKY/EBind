using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EBind.MethodDelegates;
using Xunit;

namespace EBind.Tests
{
    public class MethodDelegateCacheTest
    {
        [Fact]
        public void FinAndInvokeValueTypes()
        {
            var obj = new ValueMethodSample();

            var method = obj.GetType().GetMethod(nameof(obj.Function));
            var del = MethodDelegateCache.Find(method, method.ReflectedType);
            Assert.NotNull(del);

            del.Invoke(obj, 42, true, 42f, 42d);
        }

        private class ValueMethodSample
        {
            public virtual int Function(int a1, bool a2, float a3, double a4) => default;
        }

        [Theory]
        [MemberData(nameof(ClassMethodSamples.All), MemberType = typeof(ClassMethodSamples))]
        [MemberData(nameof(StructMethodSamples.All), MemberType = typeof(StructMethodSamples))]
        public void FindAndInvoke(object sampleObj, MethodInfo methodInfo)
        {
            var del = MethodDelegateCache.Find(methodInfo, methodInfo.ReflectedType);
            Assert.NotNull(del);

            var args = methodInfo.GetParameters().Select(p => p.Name).Prepend(sampleObj).ToArray();
            del.Invoke(args);
        }

        private class ClassMethodSamples
        {
            public static IEnumerable<object[]> All()
            {
                var sampleObj = new ClassMethodSamples();

                return typeof(ClassMethodSamples)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName)
                    .Select(m => new object[] { sampleObj, m });
            }

            // virtual for testing iOS AOT

            public virtual void Action() { }
            public virtual void Action(string one) { }
            public virtual void Action(string one, string two) { }
            public virtual void Action(string one, string two, string three) { }
            public virtual void Action(string one, string two, string three, string four) { }

            public virtual string Function() => string.Empty;
            public virtual string Function(string one) => string.Empty;
            public virtual string Function(string one, string two) => string.Empty;
            public virtual string Function(string one, string two, string three) => string.Empty;
            public virtual string Function(string one, string two, string three, string four) => string.Empty;
        }

        private struct StructMethodSamples
        {
            public static IEnumerable<object[]> All()
            {
                var sampleObj = new StructMethodSamples();

                return typeof(StructMethodSamples)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName)
                    .Select(m => new object[] { sampleObj, m });
            }

            public void Action() { }
            public void Action(string one) { }
            public void Action(string one, string two) { }
            public void Action(string one, string two, string three) { }
            public void Action(string one, string two, string three, string four) { }

            public string Function() => string.Empty;
            public string Function(string one) => string.Empty;
            public string Function(string one, string two) => string.Empty;
            public string Function(string one, string two, string three) => string.Empty;
            public string Function(string one, string two, string three, string four) => string.Empty;
        }
    }
}
