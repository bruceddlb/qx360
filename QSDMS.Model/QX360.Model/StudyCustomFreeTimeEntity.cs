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
    /// 数据表实体类：StudyCustomFreeTimeEntity 
    /// </summary>
    [Serializable()]
    public partial class StudyCustomFreeTimeEntity:BaseModel
    {    
		            
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string StudyCustomFreeTimeId { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string StudyFreeDateId { get; set; }

                    
		/// <summary>
		/// varchar:
		/// </summary>	
                 
		public string TimeSection { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? FreeStatus { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? TimeType { get; set; }

                    
		/// <summary>
		/// int:
		/// </summary>	
                 
		public int? SortNum { get; set; }

           
    }    
}
	