﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscalonamentoHospitalar.Models;

namespace EscalonamentoHospitalar.Controllers
{
    public class HorarioMedicosController : Controller
    {
        private readonly HospitalDbContext _context;

        public HorarioMedicosController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: HorarioMedicos
        public async Task<IActionResult> Index()
        {
            var hospitalDbContext = _context.HorariosMedicos.Include(h => h.Medico).Include(h => h.Turno);
            return View(await hospitalDbContext.ToListAsync());
        }

        // GET: HorarioMedicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horarioMedico = await _context.HorariosMedicos
                .Include(h => h.Medico)
                .Include(h => h.Turno)
                .FirstOrDefaultAsync(m => m.HorarioMedicoId == id);
            if (horarioMedico == null)
            {
                return NotFound();
            }

            return View(horarioMedico);
        }

        // GET: HorarioMedicos/Create
        public IActionResult Create()
        {
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "CC");
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId");
            return View();
        }

        // POST: HorarioMedicos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HorarioMedicoId,DataInicioTurno,Duracao,DataFimTurno,TurnoId,MedicoId")] HorarioMedico horarioMedico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(horarioMedico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "CC", horarioMedico.MedicoId);
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId", horarioMedico.TurnoId);
            return View(horarioMedico);
        }

        // GET: HorarioMedicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horarioMedico = await _context.HorariosMedicos.FindAsync(id);
            if (horarioMedico == null)
            {
                return NotFound();
            }
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "CC", horarioMedico.MedicoId);
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId", horarioMedico.TurnoId);
            return View(horarioMedico);
        }

        // POST: HorarioMedicos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HorarioMedicoId,DataInicioTurno,Duracao,DataFimTurno,TurnoId,MedicoId")] HorarioMedico horarioMedico)
        {
            if (id != horarioMedico.HorarioMedicoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(horarioMedico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HorarioMedicoExists(horarioMedico.HorarioMedicoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "CC", horarioMedico.MedicoId);
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId", horarioMedico.TurnoId);
            return View(horarioMedico);
        }

        // GET: HorarioMedicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horarioMedico = await _context.HorariosMedicos
                .Include(h => h.Medico)
                .Include(h => h.Turno)
                .FirstOrDefaultAsync(m => m.HorarioMedicoId == id);
            if (horarioMedico == null)
            {
                return NotFound();
            }

            return View(horarioMedico);
        }

        // POST: HorarioMedicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var horarioMedico = await _context.HorariosMedicos.FindAsync(id);
            _context.HorariosMedicos.Remove(horarioMedico);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HorarioMedicoExists(int id)
        {
            return _context.HorariosMedicos.Any(e => e.HorarioMedicoId == id);
        }

        // GET: HorarioEnfermeiro/GerarHorarioEnfermeiro
        public IActionResult GerarHorarioMedico()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GerarHorarioMedico([Bind("NumeroPessoasTurno1, NumeroPessoasTurno2, DataInicioSemana")] GerarHorarioMedico gerarHorarioMedico)
        {
            //Variáveis
            int numPessoasT1 = gerarHorarioMedico.NumeroPessoasTurno1;
            int numPessoasT2 = gerarHorarioMedico.NumeroPessoasTurno2;

            DateTime dataInicio = gerarHorarioMedico.DataInicioSemana;

            int ano = dataInicio.Year;
            int mes = dataInicio.Month;
            int dia = dataInicio.Day;

            /**********Validações***********/

            //Validar se Data de Início de Semana é uma segunda-feira
            if (DataInicioSemanaIsNotAMonday(dataInicio) == true)
            {
                //Mensagem de erro caso os médicos por turno não sejam suficientes para gerar o horário
                ModelState.AddModelError("DataInicioSemana", "Tem de selecionar uma data correspondente a uma segunda-feira e/ou igual ou superior à data atual");
            }

            //Validar Numero de Médicos por Turno
            if (NumMedicosPorTurnoIsInvalid(numPessoasT1, numPessoasT2) == true)
            {
                //Mensagem de erro caso os médicos por turno não sejam suficientes para gerar o horário
                ModelState.AddModelError("NumeroPessoasTurno3", "Não tem médicos suficientes para todos os turnos. Por favor, verifique os campos e tente novamente");
            }

            if (ModelState.IsValid)
            {
                if (!DataInicioSemanaIsNotAMonday(dataInicio) || !NumMedicosPorTurnoIsInvalid(numPessoasT1, numPessoasT2))
                {
                    //Função que insere registo na BD
                    GenerateHorarioMedico(_context, numPessoasT1, numPessoasT2, ano, mes, dia);
                    TempData["Success"] = "Horário gerado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(gerarHorarioMedico);
        }

        /************************Funções Auxiliares*************************/

        /**
        * @param db
        * @param numT1
        * @param numT2
        * @param ano
        * @param mes
        * @param dia
        * @generate a random shcedule for the doctors
        */
        private void GenerateHorarioMedico(HospitalDbContext db, int numT1, int numT2, int ano, int mes, int dia)
        {
            //Variáveis
            int numPessoasT1 = 1;
            int numPessoasT2 = 1;

            int segunda = 2;
            int sexta = 6;

            int[] medicos = MedicosIds();

            int medT1 = 0;
            int medT2 = 0;

            //Lista médicos
            List<int> listaMedicos;

            Random rnd = new Random();

            DateTime data;

            for (int i = segunda; i <= sexta; i++) //para cada dia da semana
            {
                listaMedicos = new List<int>(medicos);

                for (int j = 0; j < numPessoasT1; j++) //para cada nPessoas do Turno 1
                {
                    string turno = "MANHÃ-2";
                    int duracao = 8;

                    //escolhe aleatoriamente um médicos para o turno
                    medT1 = listaMedicos[rnd.Next(0, listaMedicos.Count())];
                    data = new DateTime(ano, mes, dia, 9, 0, 0);

                    Turno turnoId = _context.Turnos.SingleOrDefault(t => t.Nome.Equals(turno));
                    Medico medicoIdT1 = _context.Medicos.SingleOrDefault(m => m.MedicoId == medT1);

                    //Adicionar função para inserir na BD
                    InsertDataIntoHorarioMedico(db, data.AddDays(i - 2), duracao, data.AddDays(i - 2).AddHours(duracao), turnoId, medicoIdT1);

                    //remove da lista o médicos do turno
                    listaMedicos.Remove(medT1);
                }
               
                for (int k = 0; k < numPessoasT2; k++) // para cada nPesosas do Turno 2
                {
                    string turno = "TARDE";
                    int duracao = 8;

                    medT2 = listaMedicos[rnd.Next(0, listaMedicos.Count())];
                    data = new DateTime(ano, mes, dia, 16, 0, 0);

                    Turno turnoId = _context.Turnos.SingleOrDefault(t => t.Nome.Equals(turno));
                    Medico medicoIdT2 = _context.Medicos.SingleOrDefault(m => m.MedicoId == medT2);

                    //Adicionar função para inserir na BD
                    InsertDataIntoHorarioMedico(db, data.AddDays(i - 2), duracao, data.AddDays(i - 2).AddHours(duracao), turnoId, medicoIdT2);

                    //remove da lista o médico do turno
                    listaMedicos.Remove(medT2);
                }

            }
        }

        /**
        * @param db
        * @param dataInicioTurno
        * @param duracao
        * @param dataFimTurno
        * @param turnoId
        * @param medicoId
        * @insert in the HorarioMedico table a record with the above parameters
        */
        private void InsertDataIntoHorarioMedico(HospitalDbContext db, DateTime dataInicioTurno, int duracao, DateTime dataFimTurno, Turno turnoId, Medico medicoId)
        {
            db.HorariosMedicos.Add(
                new HorarioMedico { DataInicioTurno = dataInicioTurno, Duracao = duracao, DataFimTurno = dataInicioTurno.AddHours(duracao), TurnoId = turnoId.TurnoId, MedicoId = medicoId.MedicoId }
            );

            db.SaveChanges();
        }

        /**
        * @param data
        * @return true if the day of the selected date isn't a Monday
        */
        private bool DataInicioSemanaIsNotAMonday(DateTime data)
        {
            bool IsInvalid = false;
            DateTime dateNow = DateTime.Now;

            int dateTimeCompare = DateTime.Compare(data, dateNow);

            if ((data.DayOfWeek != DayOfWeek.Monday) || dateTimeCompare < 0)
            {
                IsInvalid = true;
            }

            return IsInvalid;
        }

        /**
        * @return an array with doctor's id's  
        */
        private int[] MedicosIds()
        {
            var medicos = from m in _context.Medicos
                              select m.MedicoId;

            int[] arrIdMedicos = medicos.ToArray();

            return arrIdMedicos;
        }

        /**
        * @param numT1
        * @param numT2
        * @return true if total doctors is less than sum of numT1 + numT2
        */
        private bool NumMedicosPorTurnoIsInvalid(int numT1, int numT2)
        {
            bool IsInvalid = false;

            int totalMed = numT1 + numT2;

            if (MedicosIds().Length <= totalMed)
            {
                IsInvalid = true;
            }

            return IsInvalid;
        }


    }
}
