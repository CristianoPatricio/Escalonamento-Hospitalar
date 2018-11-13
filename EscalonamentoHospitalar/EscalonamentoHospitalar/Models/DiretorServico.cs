﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscalonamentoHospitalar.Models
{
    public class DiretorServico
    {
        public int DiretorServicoID { get; set; }

        [Required(ErrorMessage = "Por favor, introduza o nome")]
        [RegularExpression(@"([A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ\s]+)", ErrorMessage = "Nome inválido")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Por favor, introduza o número mecanográfico")]
        [RegularExpression(@"[D]\d+", ErrorMessage = "Número errado")]
        public string NumeroMecanografico { get; set; }

        [RegularExpression(@"(2\d{8})|(9[1236]\d{7})", ErrorMessage = "Contacto inválido")]
        public string Contacto { get; set; }

        [Required(ErrorMessage = "Por favor, introduza o email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Por favor, introduza o nº de CC/BI")]
        [RegularExpression(@"\d{8}(\s\d{1})?", ErrorMessage = "Nº de CC/BI inválido")]
        public string CC { get; set; }

        [RegularExpression(@"([A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ\s]+)", ErrorMessage = "Morada Inválida")]
        public string Morada { get; set; }


        }
    }