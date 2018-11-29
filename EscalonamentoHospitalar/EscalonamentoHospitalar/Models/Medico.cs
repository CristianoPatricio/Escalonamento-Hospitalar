﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EscalonamentoHospitalar.Models
{
    public class Medico
    {
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "Por favor introduza o número mecanográfico respetivo")]
        [RegularExpression(@"[M]\d+", ErrorMessage = "Número mecanográfico inválido")]
        //Numero da Ordem
        public string NumeroMecanografico { get; set; }

        [Required(ErrorMessage = "Por favor introduza o nome")]
        [RegularExpression(@"([A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ\s]+)", ErrorMessage = "O nome que introduziu não é válido")]
        public string Nome { get; set; }

        [RegularExpression(@"(\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,6})", ErrorMessage = "O email que introduziu não é válido")]
        public string Email { get; set; }

        [RegularExpression(@"(2\d{8})|(9[1236]\d{7})", ErrorMessage = "O número de contacto que introduziu não é válido")]
        public string Contacto { get; set; }

        [Required(ErrorMessage = "Por favor introduza o nº de cartão de cidadão")]
        [RegularExpression(@"(\d{8}\s\d{1}[A-Z0-9]{2}\d{1})", ErrorMessage = "O nº de cartão de cidadão que introduziu não é válido")]
        public string CC { get; set; }

        [Required(ErrorMessage = "Por favor indroduza a data de nascimento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Data_Nascimento { get; set; }

        public EspecialidadeMedico EspecialidadeMedico { get; set; }
        public int EspecialidadeMedicoId { get; set; }

        public ICollection<MedicoEspecialidade> MedicosEspecialidade { get; set; }

        [Required(ErrorMessage = "Por favor indroduza a data de inicio de serviço")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Data_Inicio_Servico { get; set; }



    }
}