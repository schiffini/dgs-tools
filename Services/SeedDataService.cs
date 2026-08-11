using DgsTool.Data;
using DgsTool.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DgsTool.Services;

public class SeedDataService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<SeedDataService> _logger;

    public const string AdminRole = "Admin";
    public const string ConsultorRole = "Consultor";

    public SeedDataService(
        ApplicationDbContext db,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<SeedDataService> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await _db.Database.MigrateAsync();

        await EnsureRoleAsync(AdminRole);
        await EnsureRoleAsync(ConsultorRole);
        await EnsureAdminUserAsync();
        await SeedBusinessDataAsync();
    }

    private async Task EnsureRoleAsync(string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
            _logger.LogInformation("Rol {Role} creado", role);
        }
    }

    private async Task EnsureAdminUserAsync()
    {
        const string email = "admin@dgstool.local";
        if (await _userManager.FindByEmailAsync(email) is not null) return;

        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, "Admin123!");
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, AdminRole);
            await _userManager.AddToRoleAsync(user, ConsultorRole);
            _logger.LogInformation("Usuario admin {Email} creado", email);
        }
    }

    private async Task SeedBusinessDataAsync()
    {
        if (await _db.ParameterValues.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var admin = "admin@dgstool.local";

        var parameters = new List<ParameterValue>
        {
            new() { Code = "TASA_DOLAR", Name = "Tasa de cambio USD", Value = "18.45", Unit = "MXN", Category = "Finanzas", DataType = ParameterDataType.Number, UpdatedBy = admin, UpdatedAt = now },
            new() { Code = "IVA_PCT", Name = "IVA aplicable", Value = "16", Unit = "%", Category = "Finanzas", DataType = ParameterDataType.Number, UpdatedBy = admin, UpdatedAt = now },
            new() { Code = "MARGEN_MIN", Name = "Margen mínimo de operación", Value = "12", Unit = "%", Category = "Operación", DataType = ParameterDataType.Number, UpdatedBy = admin, UpdatedAt = now },
            new() { Code = "HORARIO_ATENCION", Name = "Horario de atención", Value = "Lun-Vie 9:00 a 18:00", Category = "Operación", DataType = ParameterDataType.String, UpdatedBy = admin, UpdatedAt = now },
            new() { Code = "POLITICA_CREDITO", Name = "Política de crédito activa", Value = "true", Category = "Operación", DataType = ParameterDataType.Boolean, UpdatedBy = admin, UpdatedAt = now },
            new() { Code = "FECHA_CIERRE", Name = "Fecha de cierre mensual", Value = "2026-08-31", Unit = "Fecha", Category = "Finanzas", DataType = ParameterDataType.Date, UpdatedBy = admin, UpdatedAt = now },
            new() { Code = "EMAIL_CONTACTO", Name = "Correo de contacto", Value = "soporte@dgstool.local", Category = "General", DataType = ParameterDataType.String, UpdatedBy = admin, UpdatedAt = now },
            new() { Code = "NIVEL_AUTORIZACION", Name = "Nivel mínimo de autorización", Value = "3", Category = "General", DataType = ParameterDataType.Number, UpdatedBy = admin, UpdatedAt = now }
        };
        _db.ParameterValues.AddRange(parameters);

        var reports = new List<ReportDefinition>
        {
            new() { Code = "RPT_VENTAS", Name = "Reporte de ventas mensual", Description = "Ingresos por línea de negocio y vendedor.", Category = "Comercial", CreatedBy = admin, CreatedAt = now },
            new() { Code = "RPT_INVENTARIO", Name = "Inventario por almacén", Description = "Existencias y movimientos por ubicación.", Category = "Operación", CreatedBy = admin, CreatedAt = now },
            new() { Code = "RPT_CUENTAS", Name = "Estado de cuentas por cobrar", Description = "Antigüedad de saldos de clientes.", Category = "Finanzas", CreatedBy = admin, CreatedAt = now },
            new() { Code = "RPT_FLUJO", Name = "Flujo de caja", Description = "Entradas y salidas de efectivo proyectadas.", Category = "Finanzas", CreatedBy = admin, CreatedAt = now }
        };
        _db.ReportDefinitions.AddRange(reports);

        _db.InformationRequests.AddRange(new List<InformationRequest>
        {
            new() { RequesterName = "María López", Subject = "Desglose de impuestos retenidos", Description = "Solicito el detalle mensual de retenciones de IVA e ISR del último trimestre.", Status = RequestStatus.Pending, Priority = RequestPriority.High, CreatedAt = now.AddDays(-1) },
            new() { RequesterName = "Carlos Ruiz", Subject = "Acceso a historial de reportes", Description = "Requiero permisos para consultar reportes del período 2025.", Status = RequestStatus.InProgress, Priority = RequestPriority.Normal, CreatedAt = now.AddDays(-3) }
        });

        await _db.SaveChangesAsync();
        _logger.LogInformation("Datos de negocio de ejemplo creados");
    }
}
