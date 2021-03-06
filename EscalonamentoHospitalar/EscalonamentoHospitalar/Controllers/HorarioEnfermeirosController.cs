﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscalonamentoHospitalar.Models;
using System.Dynamic;
using System.Globalization;

namespace EscalonamentoHospitalar.Controllers
{
    public class HorarioEnfermeirosController : Controller
    {
        
        private const int PAGE_SIZE = 45;
        private readonly HospitalDbContext _context;

        public HorarioEnfermeirosController(HospitalDbContext context)
        {
            _context = context;
        }

        public IActionResult Error()
        {
            return View();
        }

        // GET: HorarioEnfermeiros
        public async Task<IActionResult> Index(HorariosEnfermeirosViewModel model = null, int page = 1)
        {
            string nome = null;
            DateTime? data = null;

            if (model != null && model.DataInicio != null || model.CurrentNome != null)
            {
                nome = model.CurrentNome;
                data = model.DataInicio;
                page = 1;
            }

            IQueryable<HorarioEnfermeiro> horario;
            int numHorario;     
            IEnumerable<HorarioEnfermeiro> listaHorario;
           
            if (data.HasValue && string.IsNullOrEmpty(nome)) //Pesquisa por data
            {
                int ano = data.Value.Year;
                int mes = data.Value.Month;
                int dia = data.Value.Day;

                horario = _context.HorariosEnfermeiro
                   .Where(h => h.DataInicioTurno.Year.Equals(ano) && h.DataInicioTurno.Month.Equals(mes) && h.DataInicioTurno.Day.Equals(dia));

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                    .Include(h => h.Enfermeiro)
                    .Include(h => h.Turno)
                    .OrderBy(h => h.DataInicioTurno)
                    .Skip(PAGE_SIZE * (page - 1))
                    .Take(PAGE_SIZE)
                    .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(nome) && !data.HasValue) //Pesquisa por Nome
            {
                horario = _context.HorariosEnfermeiro
                    .Where(h => h.Enfermeiro.Nome.Contains(nome.Trim()));

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                    .Include(h => h.Enfermeiro)
                    .Include(h => h.Turno)
                    .OrderBy(h => h.DataInicioTurno)
                    .Skip(PAGE_SIZE * (page - 1))
                    .Take(PAGE_SIZE)
                    .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(nome) && data.HasValue) //Pesquisa por nome e data
            {
                int ano = data.Value.Year;
                int mes = data.Value.Month;
                int dia = data.Value.Day;

                horario = _context.HorariosEnfermeiro
                    .Where(h => h.Enfermeiro.Nome.Contains(nome.Trim()) && h.DataInicioTurno.Year.Equals(ano) && h.DataInicioTurno.Month.Equals(mes) && h.DataInicioTurno.Day.Equals(dia));

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                  .Include(h => h.Enfermeiro)
                  .Include(h => h.Turno)
                  .OrderBy(h => h.DataInicioTurno)
                  .Skip(PAGE_SIZE * (page - 1))
                  .Take(PAGE_SIZE)
                  .ToListAsync();
            }
            else
            {
                horario = _context.HorariosEnfermeiro;

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                  .Include(h => h.Enfermeiro)
                  .Include(h => h.Turno)
                  .OrderBy(h => h.DataInicioTurno)
                  .Skip(PAGE_SIZE * (page - 1))
                  .Take(PAGE_SIZE)
                  .ToListAsync();
            }

            if (page > (numHorario / PAGE_SIZE) + 1)
            {
                page = 1;
            }

            if (listaHorario.Count() == 0)
            {
                TempData["NoItemsFound"] = "Não foram encontrados resultados para a sua pesquisa";
            }


            return View(
                new HorariosEnfermeirosViewModel
                {
                    HorariosEnfermeiros = listaHorario,
                    Pagination = new PagingViewModel
                    {
                        CurrentPage = page,
                        PageSize = PAGE_SIZE,
                        TotalItems = numHorario
                    },
                    CurrentNome = nome,
                    DataInicio = data
                }
            );
        }

        // GET: HorarioEnfermeiros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error));
            }

            var horarioEnfermeiro = await _context.HorariosEnfermeiro
                .Include(h => h.Enfermeiro)
                .Include(h => h.Turno)
                .FirstOrDefaultAsync(m => m.HorarioEnfermeiroId == id);
            if (horarioEnfermeiro == null)
            {
                return RedirectToAction(nameof(Error));
            }

            return View(horarioEnfermeiro);
        }

        // GET: HorarioEnfermeiros/Create
        public IActionResult Create()
        {
            ViewData["EnfermeiroId"] = new SelectList(_context.Enfermeiros, "EnfermeiroId", "CC");
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId");
            return View();
        }

        // POST: HorarioEnfermeiros/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HorarioEnfermeiroId,DataInicioTurno,Duracao,DataFimTurno,TurnoId,EnfermeiroId")] HorarioEnfermeiro horarioEnfermeiro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(horarioEnfermeiro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnfermeiroId"] = new SelectList(_context.Enfermeiros, "EnfermeiroId", "CC", horarioEnfermeiro.EnfermeiroId);
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId", horarioEnfermeiro.TurnoId);
            return View(horarioEnfermeiro);
        }

        // GET: HorarioEnfermeiros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error));
            }

            var horarioEnfermeiro = await _context.HorariosEnfermeiro.FindAsync(id);
            if (horarioEnfermeiro == null)
            {
                return RedirectToAction(nameof(Error));
            }
            ViewData["EnfermeiroId"] = new SelectList(_context.Enfermeiros, "EnfermeiroId", "CC", horarioEnfermeiro.EnfermeiroId);
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId", horarioEnfermeiro.TurnoId);
            return View(horarioEnfermeiro);
        }

        // POST: HorarioEnfermeiros/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HorarioEnfermeiroId,DataInicioTurno,Duracao,DataFimTurno,TurnoId,EnfermeiroId")] HorarioEnfermeiro horarioEnfermeiro)
        {
            if (id != horarioEnfermeiro.HorarioEnfermeiroId)
            {
                return RedirectToAction(nameof(Error));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(horarioEnfermeiro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HorarioEnfermeiroExists(horarioEnfermeiro.HorarioEnfermeiroId))
                    {
                        return RedirectToAction(nameof(Error));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnfermeiroId"] = new SelectList(_context.Enfermeiros, "EnfermeiroId", "CC", horarioEnfermeiro.EnfermeiroId);
            ViewData["TurnoId"] = new SelectList(_context.Turnos, "TurnoId", "TurnoId", horarioEnfermeiro.TurnoId);
            return View(horarioEnfermeiro);
        }

        // GET: HorarioEnfermeiros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error));
            }

            var horarioEnfermeiro = await _context.HorariosEnfermeiro
                .Include(h => h.Enfermeiro)
                .Include(h => h.Turno)
                .FirstOrDefaultAsync(m => m.HorarioEnfermeiroId == id);
            if (horarioEnfermeiro == null)
            {
                return RedirectToAction(nameof(Error));
            }

            return View(horarioEnfermeiro);
        }

        // POST: HorarioEnfermeiros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var horarioEnfermeiro = await _context.HorariosEnfermeiro.FindAsync(id);
            _context.HorariosEnfermeiro.Remove(horarioEnfermeiro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HorarioEnfermeiroExists(int id)
        {
            return _context.HorariosEnfermeiro.Any(e => e.HorarioEnfermeiroId == id);
        }

        // GET: HorarioEnfermeiro/GerarHorarioEnfermeiro
        public IActionResult GerarHorarioEnfermeiro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GerarHorarioEnfermeiro([Bind("NumeroPessoasTurno1, NumeroPessoasTurno2, NumeroPessoasTurno3, DataInicioSemana")] GerarHorarioEnfermeiro gerarHorarioEnfermeiro)
        {
            //Variáveis
            int numPessoasT1 = gerarHorarioEnfermeiro.NumeroPessoasTurno1;
            int numPessoasT2 = gerarHorarioEnfermeiro.NumeroPessoasTurno2;
            int numPessoasT3 = gerarHorarioEnfermeiro.NumeroPessoasTurno3;

            DateTime dataInicio = gerarHorarioEnfermeiro.DataInicioSemana;

            int ano = dataInicio.Year;
            int mes = dataInicio.Month;
            int dia = dataInicio.Day;

            /**********Validações***********/

            //Validar se Data de Início de Semana é uma segunda-feira
            if (DataInicioSemanaIsNotAMonday(dataInicio) == true)
            {
                //Mensagem de erro caso os enfermeiros por turno não sejam suficientes para gerar o horário
                ModelState.AddModelError("DataInicioSemana", "Tem de selecionar uma data correspondente a uma segunda-feira e/ou igual ou superior à data atual");
            }          

            //Validar Numero de Enfermeiros por Turno
            if (NumEnfermeirosPorTurnoIsInvalid(numPessoasT1, numPessoasT2, numPessoasT3) == true)
            {
                //Mensagem de erro caso os enfermeiros por turno não sejam suficientes para gerar o horário
                ModelState.AddModelError("NumeroPessoasTurno3", "Não tem enfermeiros suficientes para todos os turnos. Por favor, verifique os campos e tente novamente");
            }

            // Validar se já existem horários gerados para a data selecionada
            if (HorarioEnfermeiroJaFoiGerado(dataInicio) == true)
            {
                //Mensagem de erro caso o diretor de serviço gere um horário para uma data já existente
                ModelState.AddModelError("DataInicioSemana", "Já existe um horário gerado para a semana de "+dataInicio.Day+" a "+dataInicio.AddDays(4).ToLongDateString());
            }

            if (ModelState.IsValid)
            {
                if (!DataInicioSemanaIsNotAMonday(dataInicio) || !NumEnfermeirosPorTurnoIsInvalid(numPessoasT1, numPessoasT2, numPessoasT3) || !HorarioEnfermeiroJaFoiGerado(dataInicio))
                {
                    //Função que insere registo na BD
                    GenerateHorarioEnfermeiro(_context, numPessoasT1, numPessoasT2, numPessoasT3, ano, mes, dia);
                    TempData["Success"] = "Horário gerado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(gerarHorarioEnfermeiro);
        }


        // GET: HorarioEnfermeiro/PedidoTrocaTurnoEnfermeiro
        public async Task<IActionResult> PedidoTrocaTurnoEnfermeiro(int? idHorario1, HorariosEnfermeirosViewModel model = null, int page = 1)
        {
            ViewBag.HorarioATrocar = idHorario1;

            if (idHorario1 == null)
            {
                return NotFound();
            }

            //Select EnfermeiroID Where HorarioEnfermeiroId = idHorario1
            var idEnf = from h in _context.HorariosEnfermeiro
                        where h.HorarioEnfermeiroId == idHorario1
                        select h.EnfermeiroId;

            var dataInicio = from h in _context.HorariosEnfermeiro
                             where h.HorarioEnfermeiroId == idHorario1
                             select h.DataInicioTurno;

            string nome = null;
            DateTime? data = null;

            if (model != null && model.DataInicio != null || model.CurrentNome != null)
            {
                nome = model.CurrentNome;
                data = model.DataInicio;
                page = 1;
            }

            IQueryable<HorarioEnfermeiro> horario;
            int numHorario;
            IEnumerable<HorarioEnfermeiro> listaHorario;

            if (data.HasValue && string.IsNullOrEmpty(nome)) //Pesquisa por data
            {
                int ano = data.Value.Year;
                int mes = data.Value.Month;
                int dia = data.Value.Day;

                horario = _context.HorariosEnfermeiro
                   .Where(h => h.DataInicioTurno.Year.Equals(ano) && h.DataInicioTurno.Month.Equals(mes) && h.DataInicioTurno.Day.Equals(dia) && h.HorarioEnfermeiroId != idHorario1 && h.EnfermeiroId != idEnf.Single() && h.DataInicioTurno >= dataInicio.Single());

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                    .Include(h => h.Enfermeiro)
                    .Include(h => h.Turno)
                    .OrderBy(h => h.DataInicioTurno)
                    .Skip(PAGE_SIZE * (page - 1))
                    .Take(PAGE_SIZE)
                    .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(nome) && !data.HasValue) //Pesquisa por Nome
            {
                horario = _context.HorariosEnfermeiro
                    .Where(h => h.Enfermeiro.Nome.Contains(nome.Trim()) && h.HorarioEnfermeiroId != idHorario1 && h.EnfermeiroId != idEnf.Single() && h.DataInicioTurno >= dataInicio.Single());

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                    .Include(h => h.Enfermeiro)
                    .Include(h => h.Turno)
                    .OrderBy(h => h.DataInicioTurno)
                    .Skip(PAGE_SIZE * (page - 1))
                    .Take(PAGE_SIZE)
                    .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(nome) && data.HasValue) //Pesquisa por nome e data
            {
                int ano = data.Value.Year;
                int mes = data.Value.Month;
                int dia = data.Value.Day;

                horario = _context.HorariosEnfermeiro
                    .Where(h => h.Enfermeiro.Nome.Contains(nome.Trim()) && h.DataInicioTurno.Year.Equals(ano) && h.DataInicioTurno.Month.Equals(mes) && h.DataInicioTurno.Day.Equals(dia) && h.HorarioEnfermeiroId != idHorario1 && h.EnfermeiroId != idEnf.Single() && h.DataInicioTurno >= dataInicio.Single());

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                  .Include(h => h.Enfermeiro)
                  .Include(h => h.Turno)
                  .OrderBy(h => h.DataInicioTurno)
                  .Skip(PAGE_SIZE * (page - 1))
                  .Take(PAGE_SIZE)
                  .ToListAsync();
            }
            else
            {
                horario = _context.HorariosEnfermeiro
                     .Where(h => h.HorarioEnfermeiroId != idHorario1 && h.EnfermeiroId != idEnf.Single() && h.DataInicioTurno >= dataInicio.Single());

                numHorario = await horario.CountAsync();

                listaHorario = await horario
                  .Include(h => h.Enfermeiro)
                  .Include(h => h.Turno)
                  .OrderBy(h => h.DataInicioTurno)
                  .Skip(PAGE_SIZE * (page - 1))
                  .Take(PAGE_SIZE)
                  .ToListAsync();
            }

            if (page > (numHorario / PAGE_SIZE) + 1)
            {
                page = 1;
            }                    
                   
            return View(
                new HorariosEnfermeirosViewModel
                {
                    HorariosEnfermeiros = listaHorario,
                    Pagination = new PagingViewModel
                    {
                        CurrentPage = page,
                        PageSize = PAGE_SIZE,
                        TotalItems = numHorario
                    },
                    CurrentNome = nome,
                    DataInicio = data
                });          
        }

        //GET: HorarioEnfermeiro/SolicitarPedidoTrocaTurnoEnfermeiro
        public async Task<IActionResult> SolicitarPedidoTrocaTurnoEnfermeiro(int? idHorario1, int? idHorario2)
        {  
            if (idHorario1 == null || idHorario2 == null)
            {
                return NotFound();
            }

            var horarioEnfermeiro1 = await _context.HorariosEnfermeiro
                .Include(h => h.Enfermeiro)
                .Include(h => h.Turno)
                .FirstOrDefaultAsync(m => m.HorarioEnfermeiroId == idHorario1);

            var horarioEnfermeiro2 = await _context.HorariosEnfermeiro
                .Include(h => h.Enfermeiro)
                .Include(h => h.Turno)
                .FirstOrDefaultAsync(m => m.HorarioEnfermeiroId == idHorario2);

            if (horarioEnfermeiro1 == null || horarioEnfermeiro2 == null)
            {
                return NotFound();
            }

            ViewBag.HorarioATrocar = idHorario1;
            ViewBag.HorarioParaTroca = idHorario2;

            return View(
                
                new PedidoTrocaTurnosEnfermeiroViewModel
                {
                    horarioEnfermeiroATrocar = horarioEnfermeiro1,
                    horarioEnfermeiroParaTroca = horarioEnfermeiro2
                }
                
                );
        }

        //POST:HorarioEnfermeiro/SolicitarPedidoTrocaTurnoEnfermeiro 
        [HttpPost, ActionName("SolicitarPedidoTrocaTurnoEnfermeiro")]
        [ValidateAntiForgeryToken]
        public IActionResult SolicitarPedidoTrocaTurnoEnfermeiroConfirmed(int idHor1, int idHor2)
        {
            DateTime dataPedido = DateTime.Now;
          
            // Verifica se já existe um pedido feito com os id's dos horários
            if (PedidoTrocaTurnoJaFoiEfetudado(idHor1, idHor2) == true)
            {
                TempData["PedidoAlreadyDone"] = "Já existe um pedido feito para a troca destes horários";
                return RedirectToAction(nameof(Index));
            }

            //Select EnfermeiroID Where HorarioEnfermeiroId = idHorario1
            var idEnfRequerente = from h in _context.HorariosEnfermeiro
                                   where h.HorarioEnfermeiroId == idHor1
                                   select h.EnfermeiroId;

            HorarioEnfermeiro horarioATrocar = _context.HorariosEnfermeiro.SingleOrDefault(h => h.HorarioEnfermeiroId == idHor1);
            HorarioEnfermeiro horarioParaTroca = _context.HorariosEnfermeiro.SingleOrDefault(h => h.HorarioEnfermeiroId == idHor2);
         
            try
            {
                //Insert into HorarioATrocarEnfermeiro
                InsertDataIntoHorarioATrocarEnfermeiro(_context, horarioATrocar);

                //Insert into HorarioParaTrocaEnfermeiro
                InsertDataIntoHorarioParaTrocaEnfermeiro(_context, horarioParaTroca);
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorRequired"] = "Erro ao inserir pedido!";
                return RedirectToAction(nameof(Index));
            }

            HorarioATrocarEnfermeiro horarioATrocarId = _context.HorarioATrocarEnfermeiros.LastOrDefault(h => h.HorarioEnfermeiroId == idHor1);
            HorarioParaTrocaEnfermeiro horarioParaTrocaId = _context.HorarioParaTrocaEnfermeiros.LastOrDefault(h => h.HorarioEnfermeiroId == idHor2);

            Enfermeiro enfermeiroRequerenteId =  _context.Enfermeiros.SingleOrDefault(e => e.EnfermeiroId == idEnfRequerente.Single());

            EstadoPedidoTroca estadoPedidoTrocaId = _context.EstadoPedidoTrocas.SingleOrDefault(e => e.Nome == "Pendente");

            //Insert into PedidoTrocaTurnos Table
            try
            {
                if (!PedidoTrocaTurnoJaFoiEfetudado(idHor1, idHor2))
                {
                    InsertDataIntoPedidoTrocaTurnoEnfermeiro(_context, dataPedido, enfermeiroRequerenteId, horarioATrocarId, horarioParaTrocaId, estadoPedidoTrocaId);
                    TempData["SuccessRequired"] = "Pedido realizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorRequired"] = "Erro ao inserir pedido!";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        /*************************Funções Auxiliares************************/

        /**
        * @param db
        * @param numT1
        * @param numT2
        * @param numT3
        * @param ano
        * @param mes
        * @param dia
        * @generate a random shcedule for the nurses
        */
        private void GenerateHorarioEnfermeiro(HospitalDbContext db, int numT1, int numT2, int numT3, int ano, int mes, int dia)
        {
            //Variáveis
            int numPessoasT1 = numT1;
            int numPessoasT2 = numT2;
            int numPessoasT3 = numT3;

            int segunda = 2;
            int sexta = 6;

            int[] enfermeiros = EnfermeirosIds();
            int[] enfermeirosSemFilhos = EnfermeirosSemFilhosIds();
            int[] enfermeirosComFilhos = EnfermeirosComFilhosIds();

            int enfT1 = 0;
            int enfT2 = 0;
            int enfT3 = 0;

            int[] idEnfNoite = null;

            //Lista enfermeiros
            List<int> listaEnfermeiros = new List<int>(enfermeiros);

            //Lista de enfermeiros sem filhos
            List<int> listaEnfermeirosSemFilhos;

            //Lista de enfermeiros com filhos
            List<int> listaEnfermeirosComFilhos;

            Random rnd = new Random();

            DateTime data;

            for (int i = segunda; i <= sexta; i++) //para cada dia da semana
            {
                listaEnfermeirosSemFilhos = new List<int>(enfermeirosSemFilhos);
                listaEnfermeirosComFilhos = new List<int>(enfermeirosComFilhos);

                //remover da lista enfermeiros que fizeram noite
                if (idEnfNoite != null)
                {
                    for (int n = 0; n < numPessoasT3; n++)
                    {
                        if (listaEnfermeirosSemFilhos.Contains(idEnfNoite[n]))
                        {
                            listaEnfermeirosSemFilhos.Remove(idEnfNoite[n]);
                        }
                    }
                }

                for (int j = 0; j < numPessoasT1; j++) //para cada nPessoas do Turno 1
                {

                    string turno;
                    int duracao = 8;                   

                    if (j % 2 == 0 && listaEnfermeirosComFilhos.Count() != 0) //escolhe enfermeiros com filhos
                    {
                        //escolhe aleatoriamente um enfermeiro para o turno
                        enfT1 = listaEnfermeirosComFilhos[rnd.Next(0, listaEnfermeirosComFilhos.Count())];
                        data = new DateTime(ano, mes, dia, 9, 0, 0);
                        turno = "MANHÃ-2";
                    }
                    else //escolhe enfermeiros sem filhos
                    {
                        //escolhe aleatoriamente um enfermeiro para o turno
                        enfT1 = listaEnfermeirosSemFilhos[rnd.Next(0, listaEnfermeirosSemFilhos.Count())];           
                        data = new DateTime(ano, mes, dia, 8, 0, 0);
                        turno = "MANHÃ-1";
                    }

                    Turno turnoId = _context.Turnos.SingleOrDefault(t => t.Nome.Equals(turno));
                    Enfermeiro enfermeiroIdT1 = _context.Enfermeiros.SingleOrDefault(e => e.EnfermeiroId == enfT1);

                    //Adicionar função para inserir na BD
                    InsertDataIntoHorarioEnfermeiro(db, data.AddDays(i - 2), duracao, data.AddDays(i - 2).AddHours(duracao), turnoId, enfermeiroIdT1);

                    //remove da lista o enfermeiro do turno
                    listaEnfermeirosComFilhos.Remove(enfT1);
                    //remove da lista o enfermeiro do turno
                    listaEnfermeirosSemFilhos.Remove(enfT1);
                }

                //adicionar enfermeiros que fizeram noite novamente à lista
                if (idEnfNoite != null)
                {
                    for (int n = 0; n < numPessoasT3; n++)
                    {
                        if (!listaEnfermeirosSemFilhos.Contains(idEnfNoite[n]))
                        {
                            listaEnfermeirosSemFilhos.Add(idEnfNoite[n]);
                        }
                    }
                }         

                if (i != sexta)
                {
                    for (int k = 0; k < numPessoasT2; k++) // para cada nPesosas do Turno 2
                    {
                        string turno = "TARDE";

                        Turno turnoId = _context.Turnos.SingleOrDefault(t => t.Nome.Equals(turno));

                        int duracao = 8;

                        if (k % 2 == 0 && listaEnfermeirosComFilhos.Count() != 0)
                        {
                            enfT2 = listaEnfermeirosComFilhos[rnd.Next(0, listaEnfermeirosComFilhos.Count())];
                        }
                        else
                        {
                            enfT2 = listaEnfermeirosSemFilhos[rnd.Next(0, listaEnfermeirosSemFilhos.Count())];
                        }

                        data = new DateTime(ano, mes, dia, 16, 0, 0);

                        Enfermeiro enfermeiroIdT2 = _context.Enfermeiros.SingleOrDefault(e => e.EnfermeiroId == enfT2);

                        //Adicionar função para inserir na BD
                        InsertDataIntoHorarioEnfermeiro(db, data.AddDays(i - 2), duracao, data.AddDays(i - 2).AddHours(duracao), turnoId, enfermeiroIdT2);

                        //remove da lista o enfermeiro do turno
                        listaEnfermeirosComFilhos.Remove(enfT2);
                        //remove da lista o enfermeiro do turno
                        listaEnfermeirosSemFilhos.Remove(enfT2);
                    }
                }
 
                //Reinicialização do array idEnfNoite
                idEnfNoite = new int[numPessoasT3];

                if (i != sexta)
                {
                    for (int l = 0; l < numPessoasT3; l++) // para cada nPessoas do Turno 3
                    {
                        string turno = "NOITE";

                        Turno turnoId = _context.Turnos.SingleOrDefault(t => t.Nome.Equals(turno));

                        int duracao = 8;

                        enfT3 = listaEnfermeirosSemFilhos[rnd.Next(0, listaEnfermeirosSemFilhos.Count())];
                        idEnfNoite[l] = enfT3;

                        data = new DateTime(ano, mes, dia + 1, 0, 0, 0);

                        Enfermeiro enfermeiroIdT3 = _context.Enfermeiros.SingleOrDefault(e => e.EnfermeiroId == enfT3);

                        //Adicionar função para inserir na BD
                        InsertDataIntoHorarioEnfermeiro(db, data.AddDays(i - 2), duracao, data.AddDays(i - 2).AddHours(duracao), turnoId, enfermeiroIdT3);                 

                        //remove da lista o enfermeiro do turno
                        listaEnfermeirosSemFilhos.Remove(enfT3);
                    }
                }
            }      
        }

        /**
        * @return an array with nurse's id's  
        */
        private int[] EnfermeirosIds()
        {
            var enfermeiros = from e in _context.Enfermeiros
                              select e.EnfermeiroId;

            int[] arrIdEnfermeiros = enfermeiros.ToArray();

            return arrIdEnfermeiros;
        }

        /**
        * @return an array with nurse's id's who have son's
        */
        private int[] EnfermeirosComFilhosIds()
        {
            var enfermeirosComFilhos = from e in _context.Enfermeiros
                                       where e.Filhos == true
                                       select e.EnfermeiroId;

            int[] arrIdEnfermeirosComFilhos = enfermeirosComFilhos.ToArray();

            return arrIdEnfermeirosComFilhos;
        }

        /**
        * @return an array with nurse's id's who haven't son's
        */
        private int[] EnfermeirosSemFilhosIds()
        {
            var enfermeirosSemFilhos = from e in _context.Enfermeiros
                                       where e.Filhos == false 
                                       select e.EnfermeiroId;

            int[] arrIdEnfermeirosSemFilhos = enfermeirosSemFilhos.ToArray();

            return arrIdEnfermeirosSemFilhos;
        }

        /**
        * @param db
        * @param dataInicioTurno
        * @param duracao
        * @param dataFimTurno
        * @param turnoId
        * @param enfermeiroId
        * @insert in the HorarioEnfermeiro table a record with the above parameters
        */
        private void InsertDataIntoHorarioEnfermeiro(HospitalDbContext db, DateTime dataInicioTurno, int duracao, DateTime dataFimTurno, Turno turnoId, Enfermeiro enfermeiroId)
        {
            db.HorariosEnfermeiro.Add(
                new HorarioEnfermeiro { DataInicioTurno = dataInicioTurno, Duracao = duracao, DataFimTurno = dataInicioTurno.AddHours(duracao), TurnoId = turnoId.TurnoId, EnfermeiroId = enfermeiroId.EnfermeiroId }
            );

            db.SaveChanges();
        }

        /**
        * @param db
        * @param dataPedido
        * @param enfermeiroRequerente
        * @param horarioATrocarId
        * @param horarioParaTrocaId
        * @param estadoPedidoTrocaId
        * @insert in the PedidoTrocaTurnoEnfermeiro table a record with the above parameters
        */
        private void InsertDataIntoPedidoTrocaTurnoEnfermeiro(HospitalDbContext db, DateTime dataPedido, Enfermeiro enfermeiroRequerenteId, HorarioATrocarEnfermeiro horarioATrocarId, HorarioParaTrocaEnfermeiro horarioParaTrocaId, EstadoPedidoTroca estadoPedidoTrocaId)
        {
            db.PedidoTrocaTurnosEnfermeiros.Add(

                new PedidoTrocaTurnosEnfermeiro {DataPedido = dataPedido, EnfermeiroId = enfermeiroRequerenteId.EnfermeiroId, HorarioATrocarEnfermeiroId = horarioATrocarId.HorarioATrocarEnfermeiroId, HorarioParaTrocaEnfermeiroId = horarioParaTrocaId.HorarioParaTrocaEnfermeiroId, EstadoPedidoTrocaId = estadoPedidoTrocaId.EstadoPedidoTrocaId }

               );

            db.SaveChanges();            
        }

        /**
        * @param db
        * @param horarioATrocar
        * @insert in the HorarioATrocarEnfermeiro table a record with the above parameters
        */
        private void InsertDataIntoHorarioATrocarEnfermeiro(HospitalDbContext db, HorarioEnfermeiro horarioATrocar)
        {
            db.HorarioATrocarEnfermeiros.Add(

                new HorarioATrocarEnfermeiro { HorarioEnfermeiroId = horarioATrocar.HorarioEnfermeiroId }

                );

            db.SaveChanges();
        }

        /**
        * @param db
        * @param horarioParaTroca
        * @insert in the HorarioParaTrocaEnfermeiro table a record with the above parameters
        */
        private void InsertDataIntoHorarioParaTrocaEnfermeiro(HospitalDbContext db, HorarioEnfermeiro horarioParaTroca)
        {
            db.HorarioParaTrocaEnfermeiros.Add(

                new HorarioParaTrocaEnfermeiro { HorarioEnfermeiroId = horarioParaTroca.HorarioEnfermeiroId }

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
        * @param numT1
        * @param numT2
        * @param numT3
        * @return true if total nurses is less than sum of numT1 + numT2 + numT3
        */
        private bool NumEnfermeirosPorTurnoIsInvalid(int numT1, int numT2, int numT3)
        {
            bool IsInvalid = false;

            int totalEnf = numT1 + numT2 + numT3;

            if (EnfermeirosIds().Length <= totalEnf)
            {
                IsInvalid = true;
            }

            return IsInvalid;
        }


        /**
       * @param idHorarioATrocar
       * @param idHorarioParaTroca
       * @return true if a request with above parameters was already done.
       */
        private bool PedidoTrocaTurnoJaFoiEfetudado(int idHorarioATrocar, int idHorarioParaTroca)
        {
            bool pedidoEfetuado = false;

            var pedido = from p in _context.PedidoTrocaTurnosEnfermeiros
                         where p.HorarioATrocarEnfermeiro.HorarioEnfermeiroId == idHorarioATrocar || p.HorarioParaTrocaEnfermeiro.HorarioEnfermeiroId == idHorarioParaTroca
                         select p;

            if (pedido.Count() != 0)
            {
                pedidoEfetuado = true;
            }

            return pedidoEfetuado;
        }

        /**
         * @param date
         * @return true if a schedule with above parameter was already generated
         */
         private bool HorarioEnfermeiroJaFoiGerado(DateTime date)
        {
           
            bool horarioGerado = false;

            var horario = from h in _context.HorariosEnfermeiro
                          where h.DataInicioTurno.Day == date.Day && h.DataInicioTurno.Month == date.Month && h.DataInicioTurno.Year == date.Year
                          select h;

            if (horario.Count() != 0)
            {
                horarioGerado = true;
            }

            return horarioGerado;
        }

        /*
        private bool TrocaJaEfetuada(int idHorario)
        {
            bool JaEfetuada = false;

            var troca = from t in _context.HorarioATrocarEnfermeiros
                        where t.HorarioEnfermeiroId == idHorario
                        select t;

            if (troca.Count() != 0)
            {
                JaEfetuada = true;
            }

            return JaEfetuada;
        }
        */ 

    }
}
