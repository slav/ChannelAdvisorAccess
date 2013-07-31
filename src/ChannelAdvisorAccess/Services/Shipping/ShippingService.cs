using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ChannelAdvisorAccess.Exceptions;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.ShippingService;
using Netco.Extensions;
using Netco.Logging;

namespace ChannelAdvisorAccess.Services.Shipping
{
	public class ShippingService : IShippingService
	{
		private readonly APICredentials _credentials;
		private readonly ShippingServiceSoapClient _client;

		public ShippingService( APICredentials credentials, string accountId )
		{
			this._credentials = credentials;
			this.AccountId = accountId;
			this._client = new ShippingServiceSoapClient();
		}

		public ShippingService( APICredentials credentials, string name, string id ) : this( credentials, id )
		{
			this.Name = name;
		}

		public string Name { get; private set; }
		public string AccountId { get; private set; }

		#region Ping
		public void Ping()
		{
			AP.Query.Do( () =>
				{
					var result = this._client.Ping( this._credentials );
					CheckCaSuccess( result );
				} );
		}

		public async Task PingAsync()
		{
			await AP.QueryAsync.Do( async () =>
				{
					var result = await this._client.PingAsync( this._credentials );
					CheckCaSuccess( result.PingResult );
				} );
		}
		#endregion

		#region Mark Order Shipped
		public void MarkOrderShipped( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				AP.Submit.Do( () =>
					{
						var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreateShipmentByOrderId( orderId, carrierCode, classCode, trackingNumber, dateShipped ) );
						CheckCaSuccess( result );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				await AP.SubmitAsync.Do( async () =>
					{
						var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreateShipmentByOrderId( orderId, carrierCode, classCode, trackingNumber, dateShipped ) );
						CheckCaSuccess( result.SubmitOrderShipmentListResult );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		private static OrderShipmentList CreateShipmentByOrderId( int orderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			return new OrderShipmentList
				{
					ShipmentList = new[]
						{
							new OrderShipment
								{
									OrderId = orderId,
									ShipmentType = "Full",
									FullShipment = new FullShipmentContents
										{
											carrierCode = carrierCode,
											classCode = classCode,
											dateShippedGMT = dateShipped.ToUniversalTime(),
											trackingNumber = trackingNumber
										}
								}
						}
				};
		}

		public void MarkOrderShipped( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				AP.Submit.Do( () =>
					{
						var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreateShipmentByClientId( clientOrderId, carrierCode, classCode, trackingNumber, dateShipped ) );
						CheckCaSuccess( result );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			try
			{
				await AP.SubmitAsync.Do( async () =>
					{
						var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreateShipmentByClientId( clientOrderId, carrierCode, classCode, trackingNumber, dateShipped ) );
						CheckCaSuccess( result.SubmitOrderShipmentListResult );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, carrierCode, classCode, trackingNumber, e );
			}
		}

		private static OrderShipmentList CreateShipmentByClientId( string clientOrderId, string carrierCode, string classCode, string trackingNumber, DateTime dateShipped )
		{
			return new OrderShipmentList
				{
					ShipmentList = new[]
						{
							new OrderShipment
								{
									ClientOrderIdentifier = clientOrderId,
									ShipmentType = "Full",
									FullShipment = new FullShipmentContents
										{
											carrierCode = carrierCode,
											classCode = classCode,
											dateShippedGMT = dateShipped.ToUniversalTime(),
											trackingNumber = trackingNumber
										}
								}
						}
				};
		}

		public void MarkOrderShipped( int orderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				AP.Submit.Do( () =>
					{
						var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreatePartialShipmentByOrderId( orderId, partialShipmentContents ) );
						CheckCaSuccess( result );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, partialShipmentContents.carrierCode, partialShipmentContents.classCode, partialShipmentContents.trackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( int orderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				await AP.SubmitAsync.Do( async () =>
					{
						var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreatePartialShipmentByOrderId( orderId, partialShipmentContents ) );
						CheckCaSuccess( result.SubmitOrderShipmentListResult );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( orderId.ToString( CultureInfo.InvariantCulture ), this.AccountId, partialShipmentContents.carrierCode, partialShipmentContents.classCode, partialShipmentContents.trackingNumber, e );
			}
		}

		private static OrderShipmentList CreatePartialShipmentByOrderId( int orderId, PartialShipmentContents partialShipmentContents )
		{
			return new OrderShipmentList
				{
					ShipmentList = new[]
						{
							new OrderShipment
								{
									OrderId = orderId,
									ShipmentType = "Partial",
									PartialShipment = partialShipmentContents
								}
						}
				};
		}

		public void MarkOrderShipped( string clientOrderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				AP.Submit.Do( () =>
					{
						var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId, CreatePartialShipmentByClientId( clientOrderId, partialShipmentContents ) );
						CheckCaSuccess( result );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, partialShipmentContents.carrierCode, partialShipmentContents.classCode, partialShipmentContents.trackingNumber, e );
			}
		}

		public async Task MarkOrderShippedAsync( string clientOrderId, PartialShipmentContents partialShipmentContents )
		{
			try
			{
				await AP.SubmitAsync.Do( async () =>
					{
						var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId, CreatePartialShipmentByClientId( clientOrderId, partialShipmentContents ) );
						CheckCaSuccess( result.SubmitOrderShipmentListResult );
					} );
			}
			catch( Exception e )
			{
				throw new MarkOrderShippedException( clientOrderId, this.AccountId, partialShipmentContents.carrierCode, partialShipmentContents.classCode, partialShipmentContents.trackingNumber, e );
			}
		}

		private static OrderShipmentList CreatePartialShipmentByClientId( string clientOrderId, PartialShipmentContents partialShipmentContents )
		{
			return new OrderShipmentList
				{
					ShipmentList = new[]
						{
							new OrderShipment
								{
									ClientOrderIdentifier = clientOrderId,
									ShipmentType = "Partial",
									PartialShipment = partialShipmentContents
								}
						}
				};
		}
		#endregion

		public void SubmitOrderShipmentList( IEnumerable< OrderShipment > orderShipments )
		{
			orderShipments.DoWithPages( 50, p => AP.Submit.Do( () =>
				{
					var result = this._client.SubmitOrderShipmentList( this._credentials, this.AccountId,
						new OrderShipmentList { ShipmentList = p.ToArray() } );
					this.CheckCaSuccess( result );
				} ) );
		}

		public async Task SubmitOrderShipmentListAsync( IEnumerable< OrderShipment > orderShipments )
		{
			await orderShipments.DoWithPagesAsync( 50, async p => await AP.SubmitAsync.Do( async () =>
				{
					var result = await this._client.SubmitOrderShipmentListAsync( this._credentials, this.AccountId,
						new OrderShipmentList { ShipmentList = p.ToArray() } );
					this.CheckCaSuccess( result.SubmitOrderShipmentListResult );
				} ) );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList )
		{
			return this.GetOrderShipmentHistoryList( orderIdList, new string[] { } );
		}

		public async Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList )
		{
			return await this.GetOrderShipmentHistoryListAsync( orderIdList, new string[] { } );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( string[] clientOrderIdentifierList )
		{
			return this.GetOrderShipmentHistoryList( new int[] { }, clientOrderIdentifierList );
		}

		public async Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( string[] clientOrderIdentifierList )
		{
			return await this.GetOrderShipmentHistoryListAsync( new int[] { }, clientOrderIdentifierList );
		}

		public OrderShipmentHistoryResponse[] GetOrderShipmentHistoryList( int[] orderIdList, string[] clientOrderIdentifierList )
		{
			return AP.Submit.Get( () =>
				{
					var result = this._client.GetOrderShipmentHistoryList( this._credentials, this.AccountId, orderIdList, clientOrderIdentifierList );
					CheckCaSuccess( result );
					return result.ResultData;
				} );
		}

		public async Task< OrderShipmentHistoryResponse[] > GetOrderShipmentHistoryListAsync( int[] orderIdList, string[] clientOrderIdentifierList )
		{
			return await AP.Submit.Get( async () =>
				{
					var result = await this._client.GetOrderShipmentHistoryListAsync( this._credentials, this.AccountId, orderIdList, clientOrderIdentifierList );
					CheckCaSuccess( result.GetOrderShipmentHistoryListResult );
					return result.GetOrderShipmentHistoryListResult.ResultData;
				} );
		}

		public ShippingCarrier[] GetShippingCarrierList()
		{
			var result = this._client.GetShippingCarrierList( this._credentials, this.AccountId );
			CheckCaSuccess( result );
			return result.ResultData;
		}

		public async Task< ShippingCarrier[] > GetShippingCarrierListAsync()
		{
			var result = await this._client.GetShippingCarrierListAsync( this._credentials, this.AccountId );
			CheckCaSuccess( result.GetShippingCarrierListResult );
			return result.GetShippingCarrierListResult.ResultData;
		}

		private void CheckCaSuccess( APIResultOfString results )
		{
			if( results.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( results.MessageCode, results.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfShipmentResponse result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
			ChannelAdvisorException exceptionToThrow = null;
			foreach( var shipmentResponse in result.ResultData )
			{
				if( !shipmentResponse.Success )
				{
					this.Log().Error( "Error encountered while marking order shipped: {0}", shipmentResponse.Message );
					if( exceptionToThrow == null )
						exceptionToThrow = new ChannelAdvisorException( shipmentResponse.Message );
				}
			}
			if( exceptionToThrow != null )
				throw exceptionToThrow;
		}

		private void CheckCaSuccess( APIResultOfArrayOfShippingCarrier result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}

		private void CheckCaSuccess( APIResultOfArrayOfOrderShipmentHistoryResponse result )
		{
			if( result.Status != ResultStatus.Success )
				throw new ChannelAdvisorException( result.MessageCode, result.Message );
		}
	}
}