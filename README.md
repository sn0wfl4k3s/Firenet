</br>
 
# &nbsp;Firenet
 
Firestore orm for .net applications. A simple easy way to manipulate firestore database and keeping the same domain driven design.  

</br>

## Installation
Download by dotnet cli:  

```   
  Install-Package Firenet  
```

## How to use

Put the data annotations in the class model and properties:  
![image](https://user-images.githubusercontent.com/30809620/120728408-3a39f900-c4b3-11eb-93c9-05eb8607b59d.png)

Create a class the implement 'FireContext', and write the jsonCredential with the path and create the fireCollections that represents the models that you want store:  
![image](https://user-images.githubusercontent.com/30809620/120728708-e4b21c00-c4b3-11eb-812a-06943586914d.png)
You can put data annotation 'CollectionName' for informate the name of collection. If doesn't have collection name attribute, the name of collection will be the name of object IFireCollection ('Users' in this example).  
![image](https://user-images.githubusercontent.com/30809620/120899388-dbd15f80-c605-11eb-838f-16fd3f0104d1.png)


Add on startup configureServices:  
![image](https://user-images.githubusercontent.com/30809620/120727866-feeafa80-c4b1-11eb-8e81-b4feab63224f.png) 

Or in console application, instance:  
![image](https://user-images.githubusercontent.com/30809620/120727951-33f74d00-c4b2-11eb-840e-c560ebcf68b2.png) 

