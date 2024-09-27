using Newtonsoft.Json.Linq;
using Spine.Foundation.Web.OpenEhr.Client;
using TakeCare.Migration.OpenEhr.CareDocumentation.Transformer.Models;

namespace Spine.Migration.OpenEhr.Loader
{
    public class OpenEhrLoader : IOpenEhrLoader
    {
        private IOpenEhrServiceAgent _openEhrServiceAgent => _lazyOpenEhrServiceAgent.Value;

        public OpenEhrConfigurations OpenEhrConfiguratoins => throw new NotImplementedException();

        private readonly Lazy<IOpenEhrServiceAgent> _lazyOpenEhrServiceAgent;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lazyOpenEhrServiceAgent"></param>
        public OpenEhrLoader(Lazy<IOpenEhrServiceAgent> lazyOpenEhrServiceAgent)
        {
            _lazyOpenEhrServiceAgent = lazyOpenEhrServiceAgent;
        }

        public async Task<TResult> Load<TInput, TResult>(TInput input)
            where TInput : class
            where TResult : class
        {

            ArgumentNullException.ThrowIfNull(input);

            var openEhrData = input as OpenEhrData;

            foreach (var composition in openEhrData.Compositions)
            {
                Console.WriteLine(composition.ToString());
            }


            var ehrId = await _openEhrServiceAgent.GetEhrId(openEhrData.PatientID, "spine");

            Console.WriteLine($"PatientId : {openEhrData.PatientID} , EhrId :  {ehrId} !");
            dynamic result;
            foreach (var composition in openEhrData.Compositions)
            {
                try
                {
                    result = await _openEhrServiceAgent.SaveComposition(composition as JObject, ehrId);
                    Console.WriteLine($"CompositionID :  {result} .................!!!");
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
               
            }
            return await Task.FromResult(new object() as TResult);


            /*
            ehrId = await _openEhrServiceAgent.GetEhrId("9c79f583-b35a-4e6f-9c5c-33733026c936", "spine");

            result = await _openEhrServiceAgent.SaveComposition(input as JObject, );
            return Task.FromResult(result);*/
        }


    }
}
