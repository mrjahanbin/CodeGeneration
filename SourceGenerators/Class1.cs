using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace SourceGenerators;



/// <summary>
/// Include="Microsoft.CodeAnalysis.CSharp"
/// این روش نتیجه ای بهم نداد!
/// </summary>
[Generator]
public class SampleIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Define the generator logic
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations =
            context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsClassDeclaration(s),
                    transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
                .Where(static c => c != null);

        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses =
            context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) =>
        {
            var (compilation, classes) = source;
            foreach (var classDeclaration in classes)
            {
                // Generate a simple class
                var sourceText = $@"
                    using System;

                    namespace GeneratedNamespace
                    {{
                        public class GeneratedClass
                        {{
                            public static void SayHello() => Console.WriteLine(""Hello from Generated Code!"");
                        }}
                    }}";

                spc.AddSource("GeneratedClass.g.cs", SourceText.From(sourceText, Encoding.UTF8));
            }
        });
    }


    public void Execute(GeneratorExecutionContext context)
    {
        // کدی برای تست اجرا شدن Generator
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
            "GEN001", "Generator Info", "SampleIncrementalGenerator executed", "Usage", DiagnosticSeverity.Info, true), null));
    }


    private static bool IsClassDeclaration(SyntaxNode node) =>
        node is ClassDeclarationSyntax;
}
