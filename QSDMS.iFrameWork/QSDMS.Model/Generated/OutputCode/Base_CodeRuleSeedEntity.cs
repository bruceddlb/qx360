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
    /// 数据表实体类：Base_CodeRuleSeedEntity 
    /// </summary>
    [Serializable()]
    public class Base_CodeRuleSeedEntity
    {    
		            
		/// <summary>
		/// varchar:编号规则种子主键
		/// </summary>	
                 
		public string RuleSeedId { get; set; }

                    
		/// <summary>
		/// varchar:编码规则主键
		/// </summary>	
                 
		public string RuleId { get; set; }

                    
		/// <summary>
		/// varchar:用户主键
		/// </summary>	
                 
		public string UserId { get; set; }

                    
		/// <summary>
		/// int:种子值
		/// </summary>	
                 
		public int? SeedValue { get; set; }

                    
		/// <summary>
		/// datetime:创建日期
		/// </summary>	
                 
		public DateTime? CreateDate { get; set; }

                    
		/// <summary>
		/// varchar:创建用户主键
		/// </summary>	
                 
		public string CreateUserId { get; set; }

                    
		/// <summary>
		/// varchar:创建用户
		/// </summary>	
                 
		public string CreateUserName { get; set; }

                    
		/// <summary>
		/// datetime:修改日期
		/// </summary>	
                 
		public DateTime? ModifyDate { get; set; }

                    
		/// <summary>
		/// varchar:修改用户主键
		/// </summary>	
                 
		public string ModifyUserId { get; set; }

                    
		/// <summary>
		/// varchar:修改用户
		/// </summary>	
                 
		public string ModifyUserName { get; set; }

           
    }    
}
	