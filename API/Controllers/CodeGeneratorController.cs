using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;




//{
//    "DomainName": "Product",
//    "Properties": "Id:int, Name:string, Price:decimal"
//}


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeGeneratorController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateCode([FromBody] CodeGenerationRequest request)
        {
            var properties = ParseProperties(request.Properties);

            GenerateDomain(request.DomainName);
            GenerateRepository(request.DomainName);
            GenerateUnitOfWork(request.DomainName);
            GenerateController(request.DomainName);
            //GenerateBlazorComponent(request.DomainName, properties);

            return Ok("Code generation completed.");
        }
        static (string Name, string Type)[] ParseProperties(string input)
        {
            var properties = input.Split(',');
            var parsedProperties = new (string Name, string Type)[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                var parts = properties[i].Split(':');
                parsedProperties[i] = (parts[0].Trim(), parts[1].Trim());
            }

            return parsedProperties;
        }
        private void GenerateDomain(string domainName)
        {
            var sb = "namespace CodeGeneration.Domains;\r\npublic class Product\r\n{\r\n    public int Id { get; set; }\r\n    public string Name { get; set; }\r\n    public decimal Price { get; set; }\r\n}";

            WriteToFile(Path.Combine("Domains", domainName + ".cs"), sb.ToString());
        }
        static void GenerateRepository(string domainName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using CodeGeneration.Domains;");
            sb.AppendLine();
            sb.AppendLine("namespace CodeGeneration.Repositories");
            sb.AppendLine("{");
            sb.AppendLine("    public interface I" + domainName + "Repository");
            sb.AppendLine("    {");
            sb.AppendLine("        " + domainName + " GetById(int id);");
            sb.AppendLine("        IEnumerable<" + domainName + "> GetAll();");
            sb.AppendLine("        void Add(" + domainName + " entity);");
            sb.AppendLine("        void Update(" + domainName + " entity);");
            sb.AppendLine("        void Delete(int id);");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public class " + domainName + "Repository : I" + domainName + "Repository");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly List<" + domainName + "> _" + domainName.ToLower() + "s = new List<" + domainName + ">();");
            sb.AppendLine();
            sb.AppendLine("        public " + domainName + " GetById(int id) => _" + domainName.ToLower() + "s.Find(p => p.Id == id);");
            sb.AppendLine("        public IEnumerable<" + domainName + "> GetAll() => _" + domainName.ToLower() + "s;");
            sb.AppendLine("        public void Add(" + domainName + " entity) => _" + domainName.ToLower() + "s.Add(entity);");
            sb.AppendLine("        public void Update(" + domainName + " entity)");
            sb.AppendLine("        {");
            sb.AppendLine("            var item = GetById(entity.Id);");
            sb.AppendLine("            if (item != null)");
            sb.AppendLine("            {");
            sb.AppendLine("                // Update properties here");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("        public void Delete(int id) => _" + domainName.ToLower() + "s.RemoveAll(p => p.Id == id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            WriteToFile(Path.Combine("Repositories", domainName + "Repository.cs"), sb.ToString());

        }
        static void GenerateUnitOfWork(string domainName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using CodeGeneration.Repositories;");
            sb.AppendLine();
            sb.AppendLine("namespace CodeGeneration.UnitOfWork");
            sb.AppendLine("{");
            sb.AppendLine("    public interface IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine("        I" + domainName + "Repository " + domainName + "s { get; }");
            sb.AppendLine("        void Complete();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public class UnitOfWork : IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine("        public I" + domainName + "Repository " + domainName + "s { get; } = new " + domainName + "Repository();");
            sb.AppendLine();
            sb.AppendLine("        public void Complete()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Save changes");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            WriteToFile(Path.Combine("UnitOfWork", "UnitOfWork" + ".cs"), sb.ToString());

        }
        static void GenerateController(string domainName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using CodeGeneration.Domains;");
            sb.AppendLine("using CodeGeneration.UnitOfWork;");
            sb.AppendLine();
            sb.AppendLine("namespace CodeGeneration.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    [Route(\"api/[controller]\")]");
            sb.AppendLine("    [ApiController]");
            sb.AppendLine("    public class " + domainName + "Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
            sb.AppendLine();
            sb.AppendLine("        public " + domainName + "Controller(IUnitOfWork unitOfWork)");
            sb.AppendLine("        {");
            sb.AppendLine("            _unitOfWork = unitOfWork;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine("        public IActionResult GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine("            var items = _unitOfWork." + domainName + "s.GetAll();");
            sb.AppendLine("            return Ok(items);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet(\"{id}\")]");
            sb.AppendLine("        public IActionResult GetById(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            var item = _unitOfWork." + domainName + "s.GetById(id);");
            sb.AppendLine("            if (item == null) return NotFound();");
            sb.AppendLine("            return Ok(item);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine("        public IActionResult Add(" + domainName + " item)");
            sb.AppendLine("        {");
            sb.AppendLine("            _unitOfWork." + domainName + "s.Add(item);");
            sb.AppendLine("            _unitOfWork.Complete();");
            sb.AppendLine("            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPut(\"{id}\")]");
            sb.AppendLine("        public IActionResult Update(int id, " + domainName + " item)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (id != item.Id) return BadRequest();");
            sb.AppendLine("            _unitOfWork." + domainName + "s.Update(item);");
            sb.AppendLine("            _unitOfWork.Complete();");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpDelete(\"{id}\")]");
            sb.AppendLine("        public IActionResult Delete(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            _unitOfWork." + domainName + "s.Delete(id);");
            sb.AppendLine("            _unitOfWork.Complete();");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            WriteToFile(Path.Combine("Controllers", domainName + "Controller.cs"), sb.ToString());
        }
        static void GenerateBlazorComponent(string domainName, (string Name, string Type)[] properties)
        {
            var sb = new StringBuilder();
            sb.AppendLine("@page \"/" + domainName.ToLower() + "\"");
            sb.AppendLine("@inject HttpClient Http");
            sb.AppendLine();
            sb.AppendLine("<h3>" + domainName + " Management</h3>");
            sb.AppendLine();
            sb.AppendLine("<ul>");
            sb.AppendLine("    @foreach (var item in items)");
            sb.AppendLine("    {");
            sb.AppendLine("        <li>@item.Name</li>");
            sb.AppendLine("    }");
            sb.AppendLine("</ul>");
            sb.AppendLine();
            sb.AppendLine("<h3>Add New " + domainName + "</h3>");
            sb.AppendLine();

            foreach (var property in properties)
            {
                sb.AppendLine("<label>" + property.Name + "</label>");
                sb.AppendLine("<input @bind-value=\"@" + domainName.ToLower() + "." + property.Name + "\" />");
            }

            sb.AppendLine();
            sb.AppendLine("<button @onclick=\"Add" + domainName + "\">Add</button>");
            sb.AppendLine();
            sb.AppendLine("@code {");
            sb.AppendLine("    private List<" + domainName + "> items = new List<" + domainName + ">();");
            sb.AppendLine("    private " + domainName + " " + domainName.ToLower() + " = new " + domainName + "();");
            sb.AppendLine();
            sb.AppendLine("    protected override async Task OnInitializedAsync()");
            sb.AppendLine("    {");
            sb.AppendLine("        items = await Http.GetFromJsonAsync<List<" + domainName + ">>(\"https://localhost:5001/api/" + domainName.ToLower() + "\");");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    private async Task Add" + domainName + "()");
            sb.AppendLine("    {");
            sb.AppendLine("        await Http.PostAsJsonAsync(\"https://localhost:5001/api/" + domainName.ToLower() + "\", " + domainName.ToLower() + ");");
            sb.AppendLine("        items.Add(" + domainName.ToLower() + ");");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            WriteToFile(Path.Combine("BlazorComponents", domainName + "Component.razor"), sb.ToString());
        }
        static void WriteToFile(string fileName, string content)
        {
            try
            {

                var directory = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                System.IO.File.WriteAllText(fileName, content);
                Console.WriteLine("Generated: " + fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to file " + fileName + ": " + ex.Message);
            }
        }
    }
}
