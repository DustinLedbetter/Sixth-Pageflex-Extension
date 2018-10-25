/***********************************************************************************************************************************
*                                                 GOD First                                                                        *
* Author: Dustin Ledbetter                                                                                                         *
* Release Date: 9-24-2018                                                                                                          *
* Version: 1.0                                                                                                                     *
* Purpose: To create a sixth extension for the storefront to test how they work with avalara                                       *
************************************************************************************************************************************/

using Avalara.AvaTax.RestClient;
using Pageflex.Interfaces.Storefront;
using PageflexServices;
using System;


namespace MySixthExtension
{

    public class ExtensionSix : SXIExtension
    {


        #region This is used to help shorten code throughout the program

        private ISINI GetSf()
        {
            return Storefront;
        }

        #endregion


        #region Extension Name Overides
        // At a minimum your extension must override the DisplayName and UniqueName properties.


        // The UniqueName is used to associate a module with any data that it provides to Storefront.
        public override string UniqueName
        {
            get
            {
                return "ExtensionSix.FixTax.ByAvalara.website.com";
            }
        }

        // The DisplayName will be shown on the Extensions and Site Options pages of the Administrator site as the name of your module.
        public override string DisplayName
        {
            get
            {
                return "Extension Six";
            }
        }
        #endregion


        #region This section is used to determine if we are in the "shipping" module on the storefront or not

        public override bool IsModuleType(string x)
        {

            // If we are in the shipping module return true to begin processes for this module
            if (x == "Shipping")
                return true;
            else
                return false;

        }

        #endregion


        #region This section is used to figure out the tax rates and get the zipcode entered on the shipping form

        // This method is used to get adjust the tax rate for the user's order
        public override int CalculateTax2(string OrderID, double taxableAmount, string currencyCode, string[] priceCategories, string[] priceTaxLocales, double[] priceAmount, string[] taxLocaleId, ref double[] taxAmount)
        {


            #region This section of code shows what we have been passed

            // Used to help to see where these mesages are in the Storefront logs page
            LogMessage($"*      space       *");                // Adds a space for easier viewing
            LogMessage($"*      START       *");                // Show when we start this process
            LogMessage($"*      space       *");

            // Shows what values are passed at beginning
            LogMessage(OrderID);                                // Tells the id for the order being calculated
            LogMessage(taxableAmount.ToString());               // Tells the amount to be taxed (currently set to 0)
            LogMessage(currencyCode);                           // Tells the type of currency used
            LogMessage(priceCategories.Length.ToString());      // Not Null, but empty
            LogMessage(priceTaxLocales.Length.ToString());      // Not Null, but empty
            LogMessage(priceAmount.Length.ToString());          // Not Null, but empty
            LogMessage(taxLocaleId.Length.ToString());          // Shows the number of tax locales found for this order
            LogMessage(taxLocaleId[0].ToString());              // Sends a number value which corresponds to the tax rate row in the tax rates table excel file

            #endregion


            #region This section is where we set the tax rate based on several zipcodes entered by the user

            // Shows the section where we change the tax rate
            LogMessage($"*      space        *");
            LogMessage($"*   Tax Section     *");               // Used to show where we change the tax rate
            LogMessage($"*      space        *");

            // This section saves the user's shipping info to variables to use with calculating the tax rate to return 
            // Listed in the same order as in the address book on the site
            var SFirstName = Storefront.GetValue("OrderField", "ShippingFirstName", OrderID);     // This gets the first name that the user has on the shipping page
            var SLastName = Storefront.GetValue("OrderField", "ShippingLastName", OrderID);       // This gets the last name that the user has on the shipping page
            var SAddress1 = Storefront.GetValue("OrderField", "ShippingAddress1", OrderID);       // This gets the address field 1 that the user has on the shipping page
            var SAddress2 = Storefront.GetValue("OrderField", "ShippingAddress2", OrderID);       // This gets the address field 2 that the user has on the shipping page 
            var SCity = Storefront.GetValue("OrderField", "ShippingCity", OrderID);               // This gets the city that the user has on the shipping page
            var SState = Storefront.GetValue("OrderField", "ShippingState", OrderID);             // This gets the state that the user has on the shipping page
            var SPostalCode = Storefront.GetValue("OrderField", "ShippingPostalCode", OrderID);   // This gets the zip code that the user has on the shipping page
            var SCountry = Storefront.GetValue("OrderField", "ShippingCountry", OrderID);         // This gets the country that the user has on the shipping page

            // Log to show that we have retrieved the zipcode form the shipping page
            LogMessage($"Shipping FirstName: {SFirstName}");        // This gets the first name that the user has on the shipping page
            LogMessage($"Shipping LastName: {SLastName}");          // This gets the last name that the user has on the shipping page
            LogMessage($"Shipping Address1: {SAddress1}");          // This gets the address field 1 that the user has on the shipping page
            LogMessage($"Shipping Address2: {SAddress2}");          // This gets the address field 2 that the user has on the shipping page 
            LogMessage($"Shipping City: {SCity}");                  // This gets the city that the user has on the shipping page
            LogMessage($"Shipping State: {SState}");                // This gets the state that the user has on the shipping page
            LogMessage($"Shipping PostalCode: {SPostalCode}");      // This gets the zip code that the user has on the shipping page
            LogMessage($"Shipping Country: {SCountry}");            // This gets the country that the user has on the shipping page


            // Set the tax amount based on a few zipcodes and send it back to pageflex
            LogMessage(taxAmount[0].ToString());                    // Shows the current tax amount (currently set to 0)



            

            // Create a client and set up authentication
            var client = new AvaTaxClient("MyApp", "1.0", Environment.MachineName, AvaTaxEnvironment.Production)
                .WithSecurity("AvalaraUsername", "AvalaraPassword");


            LogMessage("Client created");
            client.LogToFile("MySixthExtension\\avataxapi.log");


            
            var transaction = new TransactionBuilder(client, "AvalaraCompanyCode", DocumentType.SalesOrder, "AvalaraCustomerCode")
                        .WithAddress(TransactionAddressType.SingleLocation, SAddress1, SAddress2, null, SCity, SState, SPostalCode, SCountry)
                        .WithLine(150.0m)
                        .Create();


            //var tax = transaction.totalTax;
            decimal tax2 = transaction.totalTax ?? 0;
            LogMessage($"Your calculated tax was: {tax2}");
            // Console.WriteLine("Your calculated tax was {0}", transaction.totalTax);


            taxAmount[0] = decimal.ToDouble(tax2);              // Set our new tax rate to a value we choose (for testing) 
            LogMessage($"The zipcode was " + SPostalCode);      // Log message to inform we used this zipcode to get the amount returned
            LogMessage(taxAmount[0].ToString());                // Shows the tax amount after we changed it
            



            





            /*
            // Check if we have a few certain zipcodes to set the value to or just to use the default value
            if (SPostalCode == "37876")
            {
                // Set the new tax amount
                taxAmount[0] = 18.19;                               // Set our new tax rate to a value we choose (for testing) 
                LogMessage($"The zipcode was 37876");               // Log message to inform we used this zipcode to get the amount returned
                LogMessage(taxAmount[0].ToString());                // Shows the tax amount after we changed it
            }
            else if (SPostalCode == "12345")
            {
                // Set the new tax amount
                taxAmount[0] = 23.40;
                LogMessage($"The zipcode was 12345");
                LogMessage(taxAmount[0].ToString());
            }
            else if (SPostalCode == "01752")
            {
                // Set the new tax amount
                taxAmount[0] = 3.43;
                LogMessage($"The zipcode was 01752");
                LogMessage(taxAmount[0].ToString());
            }
            else
            {
                // Set the new tax amount
                taxAmount[0] = 13.37;
                LogMessage($"Default tax was used");
                LogMessage(taxAmount[0].ToString());
            }

            // Send message saying we have completed this process
            LogMessage($"*      space       *");
            LogMessage($"*       end        *");
            LogMessage($"*      space       *");
            */




            // Further reading if needed
            // Look at pg 436 and 467 invlovling address books if need to use them for zipcodes

            #endregion


            #region Using this will tell the storefront to calculate it's own tax using the tax tables
            // Unused, but keeping as a sidenote
            //taxAmount = null; 
            #endregion


            #region This section shows how to get the zipcode and other fields from the user's profile. 
            /*
             
            //this gets the zip code from the user profile, but we need the zip from the shipping form...
            var discountPercentageString = Storefront.GetValue("UserField", "UserProfilePricingDiscount", CurrentUserId);
            var PostalCodeString = Storefront.GetValue("UserField", "UserProfilePostalCode", CurrentUserId);

            // Used to help to see where these mesages are in the Storefront logs page
            LogMessage($"*      space       *");                // Adds a space for easier viewing
            LogMessage($"*      START       *");                // Show when we start this process
            LogMessage($"*      space       *");

            // Log the information retrieved from the user's profile fields
            LogMessage($"Current User Id: {CurrentUserId}");    // Return the user's id    
            LogMessage($"User Zipcode: {PostalCodeString}");    // Return the user's zipcode

            // Send message saying we have completed this process
            LogMessage($"*      space       *");
            LogMessage($"*       end        *");                // Return a message saying this process is finished
            LogMessage($"*      space       *");

            */
            #endregion


            return eSuccess;
        }

        #endregion


        //end of the class: ExtensionThree
    }
    //end of the file
}