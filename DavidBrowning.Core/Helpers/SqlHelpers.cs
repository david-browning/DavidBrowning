// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DavidBrowning.Helpers;

public static class SqlHelpers
{
   /// <summary>
   /// Returns true if the exception or any of its child exceptions is a 
   /// timeout exception or the database is resuming error.
   /// </summary>
   /// <param name="exception"></param>
   /// <returns></returns>
   public static bool IsWarmupRetryException(Exception exception)
   {
      // Recursively go through the inner exceptions looking for a timeout
      // or transient error.
      for (Exception? cur = exception;
         cur != null;
         cur = cur.InnerException)
      {
         if (cur is TimeoutException)
         {
            return true;
         }

         if (cur is SqlException sqlEx)
         {
            foreach (SqlError error in sqlEx.Errors)
            {
               if (IsTransientWarmupSqlError(error.Number))
               {
                  return true;
               }
            }
         }
      }

      return false;
   }

   public static bool IsTransientWarmupSqlError(int errorNumber)
   {
      return errorNumber is
         -2 or    // SQL timeout.
         40613 or // Database is not currently available / resuming.
         40197 or // Service encountered an error processing the request.
         40501 or // Service is busy.
         4060 or  // Cannot open database requested by the login.
         10928 or // Resource limit reached.
         10929;   // Resource limit reached.
   }

   public static bool IsFreeAllowanceException(Exception exception)
   {
      for (Exception? current = exception; 
         current is not null;
         current = current.InnerException)
      {
         if (current is SqlException sqlException &&
             ContainsFreeAllowancePauseMessage(sqlException))
         {
            return true;
         }
      }

      return false;
   }

   private static bool ContainsFreeAllowancePauseMessage(SqlException exception)
   {
      foreach (SqlError error in exception.Errors)
      {
         if (IsFreeAllowancePauseMessage(error.Message))
         {
            return true;
         }
      }

      return IsFreeAllowancePauseMessage(exception.Message);
   }

   private static bool IsFreeAllowancePauseMessage(string message)
   {
      return message.Contains("monthly free amount allowance", StringComparison.OrdinalIgnoreCase) &&
             message.Contains("paused for the remainder of the month", StringComparison.OrdinalIgnoreCase);
   }
}
