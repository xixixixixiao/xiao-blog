# Applied Domain-Driven Design (DDD), Part 5 - Domain Service

Domain Service is not be confused with Application Service or Web Service. Domain Service lives in the Domain Model Layer. Unlike Application or Web Service, Domain Service should not be called each time to access Domain Model Layer. You can call your repository interface in the Application Layer and get the Domain Entity directly.

## Requirement:

Your business is responsible for collecting and paying Value Added Tax (VAT) based off your business location, your customer's location, and the type of product you are selling.

## Domain Service Sample:

```cs
public class TaxDomainService : ITaxDomainService
{
    readonly IRepository<ProductTax> productTax;
    readonly IRepository<CountryTax> countryTax;
    readonly Settings settings;

    public TaxDomainService(Settings settings, IRepository<ProductTax> productTax, IRepository<CountryTax> countryTax)
    {
        this.productTax = productTax;
        this.countryTax = countryTax;
        this.settings = settings;
    }

    public decimal Calculate(Customer customer, Product product)
    {
        CountryTax customerCountryTax = this.countryTax.FindById(customer.Country.Id);
        CountryTax businessCountryTax = this.countryTax.FindById(settings.BusinessCountry.Id);
        ProductTax productTax = this.productTax.FindById(product.Code.Id);
        return (product.Cost * customerCountryTax.Percentage) + 
            (product.Cost * businessCountryTax.Percentage) + (product.Cost * productTax.Percentage);
    }
}
```

## Example usage:

```cs
Customer customer = this.repositoryCustomer.FindById(customerId);
Product product = this.repositoryProduct.FindById(productId);
 
decimal tax = this.taxDomainService.Calculate(customer, product);
 
customer.Cart.Add(CartProduct.Create(customer.Cart, product, productQuantity, tax));
```

## Summary:

Domain Service allows you to capture logic that doesn't belong in the Domain Entity.
Domain Service allows you to orchestrate between different Domain Entities.

## Tips:

Don't create too many Domain Services, most of the logic should reside in the domain entities, event handlers, etc. 
It's a great place for calculation and validation as it can access entities, and other kind of objects (e.g. Settings) that are not available via the entity graph.
Methods should return primitive types, custom enums are fine too.

**Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*
