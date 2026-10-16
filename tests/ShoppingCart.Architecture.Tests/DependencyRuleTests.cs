using NetArchTest.Rules;
using System.Reflection;

namespace ShoppingCart.Architecture.Tests
{
    public class DependencyRuleTests
    {
        private static readonly Assembly Domain = ShoppingCart.Domain.AssemblyReference.Assembly;
        private static readonly Assembly Application = ShoppingCart.Application.AssemblyReference.Assembly;
        private static readonly Assembly Infrastructure = ShoppingCart.Infrastructure.AssemblyReference.Assembly;

        [Fact]
        public void Domain_depends_on_nothing_outside_itself()
        {
            var result = Types.InAssembly(Domain).ShouldNot().HaveDependencyOnAny(
                "ShoppingCart.Application", "ShoppingCart.Infrastructure", "ShoppingCart.Api",
                "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore").GetResult();

            Assert.True(result.IsSuccessful, Failing(result));
        }

        [Fact]
        public void Application_does_not_depend_on_outer_layers_or_frameworks()
        {
            var result = Types.InAssembly(Application).ShouldNot().HaveDependencyOnAny(
                "ShoppingCart.Infrastructure", "ShoppingCart.Api",
                "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore").GetResult();

            Assert.True(result.IsSuccessful, Failing(result));
        }

        [Fact]
        public void Infrastructure_does_not_depend_on_Api()
        {
            var result = Types.InAssembly(Infrastructure).ShouldNot()
                .HaveDependencyOn("ShoppingCart.Api").GetResult();

            Assert.True(result.IsSuccessful, Failing(result));
        }

        private static string Failing(TestResult result)
            => "Violating types: " + string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>());
    }
}
