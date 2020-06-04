import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-property-pane';
import { BaseClientSideWebPart } from '@microsoft/sp-webpart-base';
 
import * as MicrosoftGraph from '@microsoft/microsoft-graph-types';

import * as strings from 'MsGraphSpFxWebPartStrings';
import MsGraphSpFx from './components/MsGraphSpFx';
import { IMsGraphSpFxProps } from './components/IMsGraphSpFxProps';

import { MSGraphClient } from '@microsoft/sp-http';

export interface IMsGraphSpFxWebPartProps {
  description: string;
}

export default class MsGraphSpFxWebPart extends BaseClientSideWebPart <IMsGraphSpFxWebPartProps> {

  public render(): void {
    this.context.msGraphClientFactory
    .getClient()
    .then(
      (client: MSGraphClient): void => {
        const element: React.ReactElement<IMsGraphSpFxProps> = React.createElement(
          MsGraphSpFx,
          { graphClient: client }
        );

        ReactDom.render(element, this.domElement);
      }
    );
  }

  protected onDispose(): void {
    ReactDom.unmountComponentAtNode(this.domElement);
  }

  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }

  protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration {
    return {
      pages: [
        {
          header: {
            description: strings.PropertyPaneDescription
          },
          groups: [
            {
              groupName: strings.BasicGroupName,
              groupFields: [
                PropertyPaneTextField('description', {
                  label: strings.DescriptionFieldLabel
                })
              ]
            }
          ]
        }
      ]
    };
  }
}
