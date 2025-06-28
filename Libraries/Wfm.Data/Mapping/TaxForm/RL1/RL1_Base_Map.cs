using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.TaxForm.RL1;


namespace Wfm.Data.Mapping.Payroll
{
    public abstract class RL1_Base_Map<T> : EntityTypeConfiguration<T> where T : RL1_Base
    {
        public RL1_Base_Map()
        {
            this.HasKey(x => x.Id);

            this.Property(x => x.SIN).HasMaxLength(9);

            this.Property(x => x.LastName).HasMaxLength(30);
            this.Property(x => x.FirstName).HasMaxLength(30);

            this.Property(x => x.EmployerAddressLine1).HasMaxLength(30);
            this.Property(x => x.EmployerAddressLine2).HasMaxLength(30);

            this.Property(x => x.AddressLine1).HasMaxLength(30);
            this.Property(x => x.AddressLine2).HasMaxLength(30);
            this.Property(x => x.PostalCode).HasMaxLength(6);

            this.Property(x => x.Box_A).HasPrecision(9, 2);
            this.Property(x => x.Box_B).HasPrecision(9, 2);
            this.Property(x => x.Box_C).HasPrecision(9, 2);
            this.Property(x => x.Box_D).HasPrecision(9, 2);
            this.Property(x => x.Box_E).HasPrecision(9, 2);
            this.Property(x => x.Box_F).HasPrecision(9, 2);
            this.Property(x => x.Box_G).HasPrecision(9, 2);
            this.Property(x => x.Box_H).HasPrecision(9, 2);
            this.Property(x => x.Box_I).HasPrecision(9, 2);
            this.Property(x => x.Box_J).HasPrecision(9, 2);
            this.Property(x => x.Box_K).HasPrecision(9, 2);
            this.Property(x => x.Box_L).HasPrecision(9, 2);
            this.Property(x => x.Box_M).HasPrecision(9, 2);
            this.Property(x => x.Box_N).HasPrecision(9, 2);
            this.Property(x => x.Box_O).HasPrecision(9, 2);
            this.Property(x => x.Box_P).HasPrecision(9, 2);
            this.Property(x => x.Box_Q).HasPrecision(9, 2);
            this.Property(x => x.Box_R).HasPrecision(9, 2);
            this.Property(x => x.Box_S).HasPrecision(9, 2);
            this.Property(x => x.Box_T).HasPrecision(9, 2);
            this.Property(x => x.Box_U).HasPrecision(9, 2);
            this.Property(x => x.Box_V).HasPrecision(9, 2);
            this.Property(x => x.Box_W).HasPrecision(9, 2);

            this.Property(x => x.Additional_Info1_Code).HasMaxLength(5);
            this.Property(x => x.Additional_Info1_Code).HasMaxLength(5);
            this.Property(x => x.Additional_Info1_Code).HasMaxLength(5);
            this.Property(x => x.Additional_Info1_Code).HasMaxLength(5);

            this.Property(x => x.Additional_Info1_Value).HasPrecision(9, 2);
            this.Property(x => x.Additional_Info2_Value).HasPrecision(9, 2);
            this.Property(x => x.Additional_Info3_Value).HasPrecision(9, 2);
            this.Property(x => x.Additional_Info4_Value).HasPrecision(9, 2);

            this.Property(x => x.EmployersQPP).HasPrecision(10, 2);
            this.Property(x => x.EmployersQPIP).HasPrecision(10, 2);
            this.Property(x => x.GrossPay).HasPrecision(18, 2);

            this.Property(x => x.Code).HasMaxLength(1);
            this.Property(x => x.Code_Case_O).HasMaxLength(2);

            this.Property(x => x.XMLSequentialNum).HasMaxLength(9);
            this.Property(x => x.LastXMLSeqNumber).HasMaxLength(9);
            this.Property(x => x.LastSequentialNum).HasMaxLength(9);
        }
    }
}

