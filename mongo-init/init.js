db = db.getSiblingDB("DynamicConfigDb");

db.ConfigRecords.insertMany([
  { Name: "SiteName", Type: "string", Value: "soty.io", IsActive: true, ApplicationName: "SERVICE-A" },
  { Name: "MaxItemCount", Type: "int", Value: "50", IsActive: true, ApplicationName: "SERVICE-A" },
  { Name: "IsBasketEnabled", Type: "bool", Value: "1", IsActive: true, ApplicationName: "SERVICE-B" }
]);
