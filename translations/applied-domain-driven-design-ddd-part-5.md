# 应用领域驱动设计, 第五章 - 领域服务 Applied Domain-Driven Design (DDD), Part 5 - Domain Service

> Domain Service is not be confused with Application Service or Web Service. Domain Service lives in the Domain Model Layer. Unlike Application or Web Service, Domain Service should not be called each time to access Domain Model Layer. You can call your repository interface in the Application Layer and get the Domain Entity directly.

不要将领域服务与应用程序服务, Web 服务弄混淆了. 领域服务位于领域模型层. 与应用程序服务或者 Web 服务不同, 不应该每次调用领域服务去访问领域模型层. 你可以在应用程序层中调用数据仓储接口, 直接获取领域实体.

## 需求 Requirement

> Your business is responsible for collecting and paying Value Added Tax (VAT) based off your business location, your customer's location, and the type of product you are selling.

你的企业负责通过 *营业地点*, *客户所在的地点* 以及 *要销售的产品类型* 来收集和支付增值税 (VAT).

## 领域服务示例 Domain Service Sample

```cs
public class TaxDomainService : ITaxDomainService
{
    readonly IRepository<ProductTax> productTax; /** 产品类型数款. */
    readonly IRepository<CountryTax> countryTax; /** 客户所在的地点税款. */
    readonly Settings settings; /** 营业地点税款. */

    public TaxDomainService(Settings settings,
                            IRepository<ProductTax> productTax,
                            IRepository<CountryTax> countryTax)
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
               (product.Cost * businessCountryTax.Percentage) +
               (product.Cost * productTax.Percentage);
    }
}
```

## 使用范例 Example usage

```cs
Customer customer = this.repositoryCustomer.FindById(customerId);
Product product = this.repositoryProduct.FindById(productId);
 
decimal tax = this.taxDomainService.Calculate(customer, product);
 
customer.Cart.Add(CartProduct.Create(customer.Cart, product, productQuantity, tax));
```

## 总结 Summary

> - Domain Service allows you to capture logic that doesn't belong in the Domain Entity.

可以通过领域服务捕获不属于领域实体的业务逻辑.

> - Domain Service allows you to orchestrate between different Domain Entities.

可以通过领域服务在不同的领域实体间进行协调.

## 技巧 Tips

> - Don't create too many Domain Services, most of the logic should reside in the domain entities, event handlers, etc.

不要创造太多的领域服务, 绝大多数的逻辑应该驻留在领域实体, 事件处理代码等中.

> - It's a great place for calculation and validation as it can access entities, and other kind of objects (e.g. Settings) that are not available via the entity graph.

- 领域服务是放置计算, 验证的最佳场所, 因为领域服务可以访问实体和通过实体图谱 (entity graph) 无法访问的其他类型的对象, 例如 `Settings`.

> - Methods should return primitive types, custom enums are fine too.

方法应该返回原始类型, 自定义枚举类型也可以.

> **Note: Code in this article is not production ready and is used for prototyping purposes only. If you have suggestions or feedback please do comment.*

**注意: 本文中的代码尚未准备好投入生产, 仅用于原型设计. 如果有建议和反馈, 请发表评论.*