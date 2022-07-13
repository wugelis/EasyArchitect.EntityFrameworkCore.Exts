using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyArchitect.EntityFrameworkCore.ExtsTests.Fake.Models
{
	[Table("HOUSINGLOANSDATAVO")]
	public class Housingloansdata
	{
		public decimal loansAmount {get; set;}


		[StringLength(50)]
		public string loansHousingLocation {get; set;}


		public DateTime? loansPeriodStart {get; set;}


		public DateTime? loansPeriosEnd {get; set;}


		[StringLength(50)]
		public string loansUse {get; set;}


		public decimal rate {get; set;}

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column(Order = 0)]
		public long loansId {get; set;}

		[Key]
		[Column(Order = 1)]
		public long customerId {get; set;}



	}

}
