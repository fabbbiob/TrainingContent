declare interface IHelloPnpWebPartStrings {
  PropertyPaneDescription: string;
  BasicGroupName: string;
  DescriptionFieldLabel: string;
}

declare module 'HelloPnpWebPartStrings' {
  const strings: IHelloPnpWebPartStrings;
  export = strings;
}
