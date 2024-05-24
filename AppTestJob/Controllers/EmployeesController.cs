using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppTestJob.Models;

namespace AppTestJob.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly DbEmployeesContext _context;

        public EmployeesController(DbEmployeesContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Verifica si la colección de empleados en el contexto 
            if (_context.Employees != null)
            {
                // Filtra los empleados activos 
                var activeEmployees = await _context.Employees.Where(e => e.IsActive == true).ToListAsync();

                // Pasa la lista de empleados 
                return View(activeEmployees);
            }
            else
            {
                // Si la colección de empleados es nula, devuelve un resultado Problem con un mensaje de error.
                return Problem("Entity set 'DbEmployeesContext.Employees' is null.");
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            //Comprobar si existe el empleado
            if (id == null || _context.Employees == null)
            {
                //Regresar que no fue encontrado en caso de no tenerlo

                return NotFound();
            }

            //Busqueda de empleado compatible con el id introducido
            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Dob,Salary,IsActive")] Employee employee)
        {
            // Validar si el nombre ya existe
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Name == employee.Name);
            if (existingEmployee != null)
            {
                ModelState.AddModelError("Name", "Employee with this name already exists.");
            }

            // Validar si todos los campos son obligatorios
            if (ModelState.IsValid)
            {
                // Validar la fecha de nacimiento (DOB)
                if (employee.Dob < new DateTime(1950, 1, 1) || employee.Dob > DateTime.Today)
                {
                    ModelState.AddModelError("Dob", "DOB must be in the range 01/01/1950 until today's date.");
                }

                // Validar el salario (Salary)
                if (employee.Salary < 100 || employee.Salary > 50000)
                {
                    ModelState.AddModelError("Salary", "Salary must be between 100 and 50000.");
                }

                // Si hay errores de validación, regresar a la vista con el modelo y los errores
                if (!ModelState.IsValid)
                {
                    return View(employee);
                }

                // Si no hay errores, guardar el empleado y redirigir a la acción Index
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, regresar a la vista con el modelo y los errores
            return View(employee);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            //Validar si el id ya existe
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            //Buscar el empleado, y comprobar si existe tal empleado
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            //Regresar el empleado 
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Dob,Salary,IsActive")] Employee employee)
        {
            // Validar si el id ya existe
            if (id != employee.Id)
            {
                return NotFound();
            }
            // Validar si el modelo es valido
            if (ModelState.IsValid)
            {

                //Actualizar el empleado que ha sido encontrado
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                //Ver si existe un error en la busqueda de error
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //Redirección hacia el Index
                return RedirectToAction(nameof(Index));
            }
            //Retorno en la vista
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            //Ver si el id introducido existe, y si los empleados dentro del contexto existen
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            //Regresar el empleado que se encontro con el ID
            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            //Si no hay, regresa null
            if (employee == null)
            {
                return NotFound();
            }
            //Regresar el componente a la vista
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Ver si existen empleados dentro de la base de datos
            if (_context.Employees == null)
            {
                return Problem("Entity set 'DbEmployeesContext.Employees'  is null.");
            }

            //Buscar empleado por ID
            var employee = await _context.Employees.FindAsync(id);
            //Si el empleado que se encontro no es nulo, eliminar el objeto 
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            //Guardar los cambios y redirigir a Index
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            //Regresar si existe un empleado con el ID
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
