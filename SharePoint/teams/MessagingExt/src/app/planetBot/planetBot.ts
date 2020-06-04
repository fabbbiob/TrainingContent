  import {
    TeamsActivityHandler,
    TurnContext,
    MessageFactory,
    CardFactory, MessagingExtensionAction, MessagingExtensionActionResponse, MessagingExtensionAttachment,
    MessagingExtensionQuery, MessagingExtensionResponse,
    AppBasedLinkQuery
  } from "botbuilder";
  
  import * as Util from "util";
  const TextEncoder = Util.TextEncoder;
  
  import * as debug from "debug";
  const log = debug("msteams");
  import * as lodash from "lodash";
  
  export class PlanetBot extends TeamsActivityHandler {
    constructor() {
      super();
    }

    protected handleTeamsMessagingExtensionFetchTask(context: TurnContext, action: MessagingExtensionAction): Promise<MessagingExtensionActionResponse> {
        // load planets & sort them by their order from the sun
        const planets: any = require("./planets.json");
        const sortedPlanets: any = lodash.sortBy(planets, ["id"])
          .map((planet) => {
            return { "value": planet.id, "title": planet.name }
          });
      
        // load card template
        const adaptiveCardSource: any = require("./planetSelectorCard.json");
        // locate the planet selector
        let planetChoiceSet: any = lodash.find(adaptiveCardSource.body, { "id": "planetSelector" });
        // update choice set with planets
        planetChoiceSet.choices = sortedPlanets;
        // load the adaptive card
        const adaptiveCard = CardFactory.adaptiveCard(adaptiveCardSource);
      
        let response: MessagingExtensionActionResponse = <MessagingExtensionActionResponse>{
          task: {
            type: "continue",
            value: {
              card: adaptiveCard,
              title: 'Planet Selector',
              height: 150,
              width: 500
            }
          }
        };
      
        return Promise.resolve(response);

    }

    protected handleTeamsMessagingExtensionSubmitAction(context: TurnContext, action: MessagingExtensionAction): Promise<MessagingExtensionActionResponse> {
        switch (action.commandId) {
          case 'planetExpanderAction':
            // load planets
            const planets: any = require("./planets.json");
            // get the selected planet
            const selectedPlanet: any = planets.filter((planet) => planet.id === action.data.planetSelector)[0];
            const adaptiveCard = this.getPlanetDetailCard(selectedPlanet);
      
            // generate the response
            return Promise.resolve(<MessagingExtensionActionResponse>{
              composeExtension: {
                type: "result",
                attachmentLayout: "list",
                attachments: [adaptiveCard]
              }
            });
            break;
          default:
            throw new Error('NotImplemented');
        }
    }

    private getPlanetDetailCard(selectedPlanet: any): MessagingExtensionAttachment {
        // load display card
        const adaptiveCardSource: any = require("./planetDisplayCard.json");
      
        // update planet fields in display card
        adaptiveCardSource.actions[0].url = selectedPlanet.wikiLink;
        lodash.find(adaptiveCardSource.body, { "id": "cardHeader" }).items[0].text = selectedPlanet.name;
        const cardBody: any = lodash.find(adaptiveCardSource.body, { "id": "cardBody" });
        lodash.find(cardBody.items, { "id": "planetSummary" }).text = selectedPlanet.summary;
        lodash.find(cardBody.items, { "id": "imageAttribution" }).text = "*Image attribution: " + selectedPlanet.imageAlt + "*";
        const cardDetails: any = lodash.find(cardBody.items, { "id": "planetDetails" });
        cardDetails.columns[0].items[0].url = selectedPlanet.imageLink;
        lodash.find(cardDetails.columns[1].items[0].facts, { "id": "orderFromSun" }).value = selectedPlanet.id;
        lodash.find(cardDetails.columns[1].items[0].facts, { "id": "planetNumSatellites" }).value = selectedPlanet.numSatellites;
        lodash.find(cardDetails.columns[1].items[0].facts, { "id": "solarOrbitYears" }).value = selectedPlanet.solarOrbitYears;
        lodash.find(cardDetails.columns[1].items[0].facts, { "id": "solarOrbitAvgDistanceKm" }).value = Number(selectedPlanet.solarOrbitAvgDistanceKm).toLocaleString();
      
        // return the adaptive card
        return CardFactory.adaptiveCard(adaptiveCardSource);
    }

    protected handleTeamsMessagingExtensionQuery(context: TurnContext, query: MessagingExtensionQuery): Promise<MessagingExtensionResponse> {
      // get the search query
      let searchQuery = "";
      if (query && query.parameters && query.parameters[0].name === "searchKeyword" && query.parameters[0].value) {
        searchQuery = query.parameters[0].value.trim().toLowerCase();
      }
    
      // load planets
      const planets: any = require("./planets.json");
      // search results
      let queryResults: string[] = [];
    
      switch (searchQuery) {
        case "inner":
          // get all planets inside asteroid belt
          queryResults = planets.filter((planet) => planet.id <= 4);
          break;
        case "outer":
          // get all planets outside asteroid belt
          queryResults = planets.filter((planet) => planet.id > 4);
          break;
        default:
          // get the specified planet
          queryResults.push(planets.filter((planet) => planet.name.toLowerCase() === searchQuery)[0]);
      }
    
      // get the results as cards
      let searchResultsCards: MessagingExtensionAttachment[] = [];
      queryResults.forEach((planet) => {
        searchResultsCards.push(this.getPlanetResultCard(planet));
      });
    
      let response: MessagingExtensionResponse = <MessagingExtensionResponse>{
        composeExtension: {
          type: "result",
          attachmentLayout: "list",
          attachments: searchResultsCards
        }
      };
    
      return Promise.resolve(response);
    }

    protected handleTeamsAppBasedLinkQuery(context: TurnContext, query: AppBasedLinkQuery): Promise<MessagingExtensionResponse> {
      // load planets
      const planets: any = require("./planets.json");
      // get the selected planet
      const selectedPlanet: any = planets.filter((planet) => planet.wikiLink === query.url)[0];
      const adaptiveCard = this.getPlanetResultCardFake(selectedPlanet);
    
      // generate the response
      return Promise.resolve(<MessagingExtensionActionResponse>{
        composeExtension: {
          type: "result",
          attachmentLayout: "list",
          attachments: [adaptiveCard]
        }
      });
    }

    private getPlanetResultCard(selectedPlanet: any): MessagingExtensionAttachment {
      return CardFactory.heroCard(selectedPlanet.name, selectedPlanet.summary, [selectedPlanet.imageLink]);
    }
    private getPlanetResultCardFake(selectedPlanet: any): MessagingExtensionAttachment {
      return CardFactory.heroCard(selectedPlanet.name, "BLAH BLA BLAH BLAH", ["https://upload.wikimedia.org/wikipedia/commons/thumb/a/aa/Logo_Google_2013_Official.svg/1280px-Logo_Google_2013_Official.svg.png"]);
    }
  }