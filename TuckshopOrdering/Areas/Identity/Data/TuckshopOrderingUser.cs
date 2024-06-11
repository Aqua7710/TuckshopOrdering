using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TuckshopOrdering.Areas.Identity.Data;

// Add profile data for application users by adding properties to the TuckshopOrderingUser class
public class TuckshopOrderingUser : IdentityUser
{
    public string FullName { get; set; }
}

