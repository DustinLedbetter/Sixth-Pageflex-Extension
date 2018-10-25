# Sixth-Pageflex-Extension
This extension checks if we are on the shipping page, prints to the logs what we have passed to calculate tax rates, retrieves all of the user's shipping information, connects to avalara and sends the data of user to retrieve tax amount (currently the price or subtotal is harcoded as can't access the actual storefronts yet). and then displays the amount back to the storefront  


This one was scrapped as an update to part of a nuget caused issues that could not be easily resolved at the time.


Methods
1. DisplayName()
2. UniqueName()
3. private ISINI GetSf () (reduces code throughout project)
4. IsModuleType (string x) (determines if at shipping step)
5. CalculateTax2 () 
