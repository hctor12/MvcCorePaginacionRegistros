using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;

#region VIEWS && SPs

/*create view V_DEPARTAMENTOS_INDIVIDUAL
as
SELECT cast (ROW_NUMBER() over (order by DEPT_NO) as int) as POSICION
, DEPT_NO, DNOMBRE, LOC from DEPT
go

create procedure SP_GRUPO_DEPARTAMENTOS(@posicion int)
as
	select DEPT_NO, DNOMBRE, LOC from V_DEPARTAMENTOS_INDIVIDUAL
	where POSICION >= @posicion and POSICION < (@posicion + 2)
go

create view V_GRUPO_EMPLEADOS
as
	select cast(ROW_NUMBER() over (order by APELLIDO) as int) POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	from EMP
go

create procedure SP_GRUPO_EMPLEADOS(@posicion int)
as
	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from V_GRUPO_EMPLEADOS WHERE POSICION >= @posicion and POSICION < (@posicion + 3)
go

create procedure SP_GRUPO_EMPLEADOS_OFICIO
(@posicion int, @oficio nvarchar(50))
as
	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from
	(select cast(row_number() over (order by APELLIDO) as int)
	POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	from EMP
	where OFICIO=@oficio ) QUERY
	where (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 3))
go

create procedure SP_GRUPO_EMPLEADOS_OFICIO
(@posicion int, @oficio nvarchar(50), @registros int OUT)
as
	select @registros = count(EMP_NO) from EMP
	where OFICIO = @oficio
	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from
	(select cast(row_number() over (order by APELLIDO) as int)
	POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	from EMP
	where OFICIO=@oficio ) QUERY
	where (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 3))
go
 */

#endregion

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento departamento = await this.context.VistaDepartamentos.Where(z => z.Posicion == posicion).FirstOrDefaultAsync();
            return departamento;
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamentoAsync(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion && datos.Posicion < (posicion + 2)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<Departamento>> GetGrupoDepartamentoAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados.Where(z => z.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosOficioAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            return await this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio).ToListAsync();
        }

        public async Task<ModelEmpleadosOficio> GetGrupoEmpleadosOficioOutAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);
            pamRegistros.Direction = ParameterDirection.Output;
            List<Empleado> empleados = await this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio, pamRegistros).ToListAsync();
            int registros = Convert.ToInt32(pamRegistros.Value);
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }

        public async Task<Departamento> FindDepartamento(int idDepartamento)
        {
            var consulta = from datos in this.context.Departamentos where datos.IdDepartamento == idDepartamento select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<int> GetEmpleadosRegistroDepartamentoCountAsync(int iddep)
        {
            return await this.context.Empleados.Where(z => z.IdDepartamento == iddep).CountAsync();
        }

        public async Task<Empleado> GetEmpleadosRegistroDepartamentoOutAsync(int posicion, int iddep)
        {
            string sql = "SP_REGISTRO_DEPARTAMENTOS @posicion, @iddep, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamIddep = new SqlParameter("@iddep", iddep);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);
            pamRegistros.Direction = ParameterDirection.Output;
            Empleado empleado = await this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamIddep, pamRegistros).AsAsyncEnumerable().FirstOrDefaultAsync();
            //int registros = Convert.ToInt32(pamRegistros.Value);
            return empleado;
        }
    }
}
