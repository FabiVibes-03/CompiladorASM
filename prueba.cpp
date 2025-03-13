using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
  int sum, i, j;
    sum = 0;
  
    for (i = 0; i < 5; i++) {
         if (i % 2 == 0)
             sum += i;
         else
             sum += (i * 2);
    }
    
    
    j = 0;
    while (j < 5) {
         sum = sum + (j * 2);
         j++;
    }
    
    
    j = 0;
    do {
         sum = sum + (j * 3);
         j++;
    } while (j < 5);
}