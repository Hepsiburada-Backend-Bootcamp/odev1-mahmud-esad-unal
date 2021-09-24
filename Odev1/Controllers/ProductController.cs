using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odev1.Properties
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private const string PRODUCT_LIST_PATH = "productList.json";
        private List<ProductDto> _productList = new List<ProductDto>();

        public ProductController()
        {
            LoadProductList();
        }

        /* Loads the product list from local json file */
        private void LoadProductList()
        {
            using (StreamReader r = new StreamReader(PRODUCT_LIST_PATH))
            {
                string json = r.ReadToEnd();
                _productList = JsonConvert.DeserializeObject<List<ProductDto>>(json);
            }
        }

        /* Adds a product to the product list */
        private void AddToProductList(ProductDto product)
        {
            using (StreamReader r = new StreamReader(PRODUCT_LIST_PATH))
            {
                string readJson = r.ReadToEnd();
                _productList = JsonConvert.DeserializeObject<List<ProductDto>>(readJson);
                _productList.Add(product);
            }
            SaveProductList();
        }

        /* Saves the updated product list to file */
        private void SaveProductList()
        {
            string json = JsonConvert.SerializeObject(_productList, Formatting.Indented);

            System.IO.File.WriteAllText(PRODUCT_LIST_PATH, json);
        }

        /* Used for equalizing attributes of two products */
        private void UpdateProductSpecs(ref ProductDto product, UpdateProductDto newProduct)
        {
            product.Category = newProduct.Category;
            product.Name = newProduct.Name;
            product.Price = newProduct.Price;
            product.Vendor = newProduct.Vendor;
        }
        

        [HttpGet]
        public IActionResult Get([FromQuery] ProductFilter filter)
        {
            bool isFilterCategory = false;
            bool isFilterVendor = false;

            if (filter.Category is not null)
            {
                isFilterCategory = true;
            }
            if(filter.Vendor is not null)
            {
                isFilterVendor = true;
            }

            /* Filter the list from query input (This part is taken directly from the web) */ 
            Predicate<ProductDto> match = product =>
                product.Category.ToString().ToLower() == (isFilterCategory ? filter.Category : product.Category.ToString().ToLower()) &&
                product.Vendor.ToLower() == (isFilterVendor ? filter.Vendor.ToLower() : product.Vendor.ToLower());

            List<ProductDto> filteredList = _productList.FindAll(match);

            return Ok(filteredList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            ProductDto product = _productList.FirstOrDefault(x => x.Id == id);

            if(product == null)
            {
                return BadRequest($"Could not find the product with id: {id}.");
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Add([FromBody] UpdateProductDto product)
        {
            int lastId = _productList.Max(x => x.Id);

            ProductDto newProduct = new ProductDto();
            UpdateProductSpecs(ref newProduct, product);

            /* Set id value*/
            newProduct.Id = lastId + 1;

            AddToProductList(newProduct);
            return Created($"Product olusturuldu (id: {lastId})", newProduct);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateProductDto product)
        {
            ProductDto oldProduct = _productList.FirstOrDefault(x => x.Id == id);

            if (oldProduct != null)
            {
                int index = _productList.IndexOf(oldProduct);

                UpdateProductSpecs(ref oldProduct, product);

                _productList[index] = oldProduct;

                SaveProductList();

                return Ok($"Updated the product with id: {id}");
            }

            return BadRequest($"Could not find the product with id: {id}.");
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            ProductDto product = _productList.FirstOrDefault(x => x.Id == id);

            if (product != null)
            {
                _productList.Remove(product);

                SaveProductList();
                return Ok($"Removed the product with id: {id}");
            }

            return BadRequest($"Could not find the product with id: {id}.");
        }
    }
}
