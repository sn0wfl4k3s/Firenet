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

Create a class that implement 'FireContext' and write the jsonCredential with the path and create the fireCollections that represents the models that you want store:  
![image](https://user-images.githubusercontent.com/30809620/120728708-e4b21c00-c4b3-11eb-812a-06943586914d.png)  

You can put data annotation 'CollectionName' for informate the name of collection. If it doesn't have collection name data annotation, the name of collection will be the name of object IFireCollection ('Users' in this example).  
![image](https://user-images.githubusercontent.com/30809620/120899679-4df67400-c607-11eb-932b-b6588bc39002.png)


Add on startup configureServices:  
![image](https://user-images.githubusercontent.com/30809620/120727866-feeafa80-c4b1-11eb-8e81-b4feab63224f.png) 

Or in console application, instance:  
![image](https://user-images.githubusercontent.com/30809620/120727951-33f74d00-c4b2-11eb-840e-c560ebcf68b2.png) 

## Last version news (1.1.0 or later)

In this last version or later, no more need the configuring for JsonCredentialsPath if you have already configured the environment variable GOOGLE_APPLICATION_CREDENTIALS in machine that is running your dotnet application. Now you just need put the constructor with base, like below:  
![image](https://user-images.githubusercontent.com/30809620/131598460-c9a5baef-78cf-4b3c-a760-c3d2f8efcc42.png)

And the instantiation or dependency injection keep the same way:  
![image](https://user-images.githubusercontent.com/30809620/131598672-c627b2d6-7a4a-4620-98ec-8f5b7508e57f.png)  

![image](https://user-images.githubusercontent.com/30809620/131598715-d3dcd2f6-87e2-4bd2-b9ce-7df471531404.png)  

But if you want set the path of your json credentials file from firestore, you should configuring like below:  
![image](https://user-images.githubusercontent.com/30809620/131599087-f383c52a-64b5-4e87-8ee3-f898c033b88c.png)  
![image](https://user-images.githubusercontent.com/30809620/131599238-3ffa196a-606e-48b1-9bd2-ecdefb3de1d0.png)  
