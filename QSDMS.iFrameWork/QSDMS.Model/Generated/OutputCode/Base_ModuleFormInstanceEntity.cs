//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由T4模板自动生成
//	   生成时间 2017-06-11 15:18:04 by 群升科技
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
    
using System;
namespace QSDMS.Model 
{
    /// <summary>
    /// 数据表实体类：Base_ModuleFormInstanceEntity 
    /// </summary>
    [Serializable()]
    public class Base_ModuleFormInstanceEntity
    {    
		            
		/// <summary>
		/// varchar:表单实例主键
		/// </summary>	
                 
		public string FormInstanceId { get; set; }

                    
		/// <summary>
		/// varchar:表单主键
		/// </summary>	
                 
		public string FormId { get; set; }

                    
		/// <summary>
		/// varchar:表单实例Json
		/// </summary>	
                 
		public string FormInstanceJson { get; set; }

                    
		/// <summary>
		/// varchar:对象主键
		/// </summary>	
                 
		public string ObjectId { get; set; }

                    
		/// <summary>
		/// int:排序码
		/// </summary>	
                 
		public int? SortCode { get; set; }

                    
		/// <summary>
		/// varchar:备注
		/// </summary>	
                 
		public string Description { get; set; }

           
    }    
}
	