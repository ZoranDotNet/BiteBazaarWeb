# ByteBazaar Web Shop
## Debug Thug's web shop application
We received a task to create an web shop and an API service that is meant to be used together. 

###The functionalites that were requested: 
#### Customer 
Be able to:
- make a purchase
- register a customer account
#### Admin 
Ability to:
- add, remove and modify the products
- add the quantity for all/one product(s)
- add different type's of temporary campaigns or discount prices for a specified time period
- see a log with all order's made
- access a dashboard with different statistics, such as:
- see number of purchases and the totalt value in SEK between set dates
- see the top ten most sold products between set dates
- see inventory balance below 5 units
### The extra functionalites that we added:
- Added demand for register/login to make a purchase
#### Customer 
Is able to:
- add product's to a favorite list
- see own order history
- see order status
- register account via google auth
- filter product's by category
- search for products
#### Admin 
Is able to:
- see product details on every order
- changes order status to Sent
- lock or unlock customer accounts
- change role from customer to admin or viceversa


#### Thank you for using Debug Thugs Application
##### PLEASE NOTE: To be able to use the admin functions you need to contact
- Angelica Lindström
- Emil Nordin
- Zoran Matovic
- Theres Sundberg Selin
Since the admin have the option to remove all products from the API we won't leave the login credentials in this ReadMe file.
If you visit our web shop as a customer or just want to try it out, just register a account or use the google login option. No order's will be billed and no product's will be shipped.
##### PLEASE NOTE NO PASSWORD'S THAT YOU USE ARE AVAILABLE FOR US TO SEE

##### Technical notes:
We built an API (see repo https://github.com/roxzlir/ByteBazaarAPI) that is published on azure with it's own database. The API contains all products, categories and images related to products. 
The ByteBazaarWeb is also published on azure with it's oen database to store all the favorite products, carts, order, account's and everything that is need to give you the ultimate shopping experience.
We have used ASP.Core with C# as base, but we have also used HTML, CSS, JavaScript for this build. 


#### The Debug Thugs Team:
##### -> Angelica Lindström
##### -> Zoran Matovic
##### -> Emil Nordin
##### -> Theres Sundberg Selin
