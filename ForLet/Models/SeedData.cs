using ForLet.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ForLetDbContext(serviceProvider.GetRequiredService<DbContextOptions<ForLetDbContext>>()))
            {
                if (context.States.Any()) // if Any states are created 
                {
                    return;
                }

                context.States.AddRange(
                    new State { StateName = "Abia" },
                    new State { StateName = "Anambra"},
                    new State { StateName = "FCT"},
                    new State { StateName = "Kwara"},
                    new State { StateName = "Lagos"}
                );
                context.SaveChanges();

                // add the L.G.As for theses states 
                context.Lgas.AddRange(
                    new LGA { LGAName = "Aba North", StateId = context.States.Single(s => s.StateName == "Abia").Id },
                    new LGA { LGAName = "Aba South", StateId = context.States.Single(s => s.StateName == "Abia").Id },
                    new LGA { LGAName = "Isiala-Ngwa North", StateId = context.States.Single(s => s.StateName == "Abia").Id },
                    new LGA { LGAName = "Fufore", StateId = context.States.Single(s => s.StateName == "Anambra").Id },
                    new LGA { LGAName = "Ganye", StateId = context.States.Single(s => s.StateName == "Anambra").Id },
                    new LGA { LGAName = "Gombi", StateId = context.States.Single(s => s.StateName == "Anambra").Id },
                    new LGA { LGAName = "Offa", StateId = context.States.Single(s => s.StateName == "Kwara").Id },
                    new LGA { LGAName = "Ilorin", StateId = context.States.Single(s => s.StateName == "Kwara").Id },
                    new LGA { LGAName = "Gwagalada", StateId = context.States.Single(s => s.StateName == "FCT").Id },
                    new LGA { LGAName = "Kuje", StateId = context.States.Single(s => s.StateName == "FCT").Id },
                    new LGA { LGAName = "Abaji", StateId = context.States.Single(s => s.StateName == "FCT").Id },
                    new LGA { LGAName = "Ojo", StateId = context.States.Single(s => s.StateName == "Lagos").Id },
                    new LGA { LGAName = "Ajeromi/Ifelodun", StateId = context.States.Single(s => s.StateName == "Lagos").Id },
                    new LGA { LGAName = "Lagos Island", StateId = context.States.Single(s => s.StateName == "Lagos").Id }
                );
                context.SaveChanges();

                context.PropertyTypes.AddRange(
                    new PropertyType { PropertyName = "Shortlet Rentals" },
                    new PropertyType { PropertyName = "3 Bedroom Flat" },
                    new PropertyType { PropertyName = "2 Bedroom Flat" },
                    new PropertyType { PropertyName = "1 Bedroom & Parlour" },
                    new PropertyType { PropertyName = "1 room(face to face)" },
                    new PropertyType { PropertyName = "Shop" },
                    new PropertyType { PropertyName = "Office" },
                    new PropertyType { PropertyName = "Land" }
                );
                context.SaveChanges();
            }
        }
    }
}
