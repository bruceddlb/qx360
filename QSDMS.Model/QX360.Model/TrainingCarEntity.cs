//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由T4模板自动生成
//	   生成时间 2018-07-18 15:21:41 by bruced
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
    
using System;
namespace QX360.Model
{
    /// <summary>
    /// 数据表实体类：TrainingCarEntity 
    /// </summary>
    [Serializable()]
    public partial class TrainingCarEntity:BaseModel
    {    
		            
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string TrainingCarId { get; set; }

                    
		/// <summary>
		/// nchar:
		/// </summary>	
                 
		public string SchoolName { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string SchoolId { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string Name { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string CarNumber { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? Status { get; set; }

                    
		/// <summary>
		/// datetime:
		/// </summary>	
                 
		public DateTime? CreateTime { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string CreateId { get; set; }

                    
		/// <summary>
		/// text:
		/// </summary>	
                 
		public string Remark { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? TrainingType { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string TrainingTypeName { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? SortNum { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string FaceImage { get; set; }

           
    }    
}
	