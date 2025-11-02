using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.RegularExpressions;

namespace UPTSiteTests;

[TestClass]
public class UPTSiteTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(
                Environment.CurrentDirectory,
                "playwright-traces",
                $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}.zip"
            )
        });
        // await Context.CloseAsync();
    }

    [TestMethod]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Universidad"));
    }

    [TestMethod]
    public async Task GetSchoolDirectorName()
    {
        // Arrange
        string schoolDirectorName = "Ing. Martha Judith Paredes Vignola";
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Act
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pre-Grado" }).HoverAsync(); //ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Escuela Profesional de Ingeniería de Sistemas" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Escuela Profesional de" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Plana Docente" }).ClickAsync();

        // Assert
        await Expect(Page.GetByText("Ing. Martha Judith Paredes")).ToContainTextAsync(schoolDirectorName);
    }

    [TestMethod]
    public async Task SearchStudentInDirectoryPage()
    {
        // Navegar al portal principal
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Cerrar popup inicial
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();

        // Navegar al directorio de estudiantes
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pre-Grado" }).HoverAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Escuela Profesional de Ingeniería de Sistemas" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Estudiantes" }).ClickAsync();

        // Buscar en el iframe
        var frame = Page.FrameLocator("iframe");
        await frame.GetByRole(AriaRole.Textbox).FillAsync("AGUILAR");
        await frame.GetByRole(AriaRole.Button, new() { Name = "Buscar" }).ClickAsync();

        // ✅ Nueva validación: que la tabla esté visible y tenga al menos un resultado
        var table = frame.GetByRole(AriaRole.Table);
        await Expect(table).ToBeVisibleAsync();
        await Expect(table).ToContainTextAsync("AGUILAR");

    }

    [TestMethod]
    public async Task OpenCampusVirtual()
    {
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Cerrar popup
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();

        // Verificar que el enlace 'Campus Virtual' exista y sea visible
        var campusLink = Page.GetByRole(AriaRole.Link, new() { Name = "Campus Virtual" });
        await Expect(campusLink).ToBeVisibleAsync();

        // Clic al enlace y esperar carga
        await campusLink.ClickAsync();

        // Validar que se haya cargado una página (sin exigir dominio exacto)
        await Expect(Page).ToHaveURLAsync(new Regex("upt\\.edu\\.pe"));
    }


    [TestMethod]
    public async Task VerifyAboutUsSection()
    {
        await Page.GotoAsync("https://www.upt.edu.pe");

        // Cerrar popup
        await Page.GetByRole(AriaRole.Button, new() { Name = "×" }).ClickAsync();

        // Clic al primer enlace que contenga "Sobre nosotros"
        await Page.GetByRole(AriaRole.Link, new() { Name = "Sobre nosotros" }).First.ClickAsync();

        // Verificar texto en la sección abierta
        await Expect(Page.Locator("body")).ToContainTextAsync("Universidad Privada de Tacna");
    }

}