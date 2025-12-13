Producto: 
GetAll ✅
GetById ✅
Create ✅
Update ✅
Delete ✅

Category:
GetAll ✅
GetById ✅
Create ✅
Update ✅
Delete ✅

Order: 
GetAll ✅
GetById ✅
Delete ✅
Create: Esto para crear la orden desde el backoffice, para ello primero se crea la Orden con los datos 
basicos ✅, luego hay que crear un endpoint que añada productos a esa orden:
1 - Primero crea el ShoppingCart  ✅
2 - Asocia ese ShoppingCart a la Orden ✅
3 - Asigna los productos a ese ShoppingCart  ✅       
ChangeStatus ✅
Update ✅
CreateOrderInLanding ✅

ShopingCart:
CreateOrAdd: Si el id del carrito está vacío creo el carrito a partir de la adición del producto,
 si tiene un item solo creo el carrito ✅
DeleteToCar: Lo mismo
UpdateToItemInCar: Un put para cambiar la cantidad del carrito
CloseCart: el close cart directamente debe crear una orden y ponerla en estado pendiente, en caso de que tenga items claro está,
de no tenerlo, se elimina el carrito

