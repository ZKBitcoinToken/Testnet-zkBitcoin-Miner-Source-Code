/*
   Copyright 2018 Lip Wee Yeo Amano

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SoliditySHA3Miner.NetworkInterface
{
    public class Web3Interface : NetworkInterfaceBase
    {
        public HexBigInteger LastSubmitGasPrice { get; private set; }

        private const int MAX_TIMEOUT = 15;

        private readonly Web3 m_web3;
        private readonly Contract m_contract;
        private readonly Account m_account;
        private readonly Function m_mintMethod;
        private readonly Function m_mintMethodwithETH;
        private readonly Function m_NFTmintMethod;
        private readonly Function m_ERC20mintMethod;
        private readonly Function m_transferMethod;
        private readonly Function m_getMiningDifficulty2;
        private readonly Function m_getMiningDifficulty;
        private readonly Function m_getETH2SEND;
        private readonly Function m_getMiningDifficulty3;
        private readonly Function m_getMiningDifficulty4;
        private readonly Function m_getEpoch;
        private readonly Function m_getMiningTarget;
        private readonly Function m_getChallengeNumber;
        private readonly Function m_getMiningReward;
        private readonly Function m_MAXIMUM_TARGET;

        private readonly Function m_CLM_ContractProgress;

        private readonly int m_mintMethodInputParamCount;

        private readonly float m_gasToMine;
        private readonly float m_gasApiMax;
        private readonly ulong m_gasLimit;

        private readonly bool m_ETHwithMints;
        private readonly string m_gasApiURL2;
        private readonly string m_gasApiPath2;
        private readonly string m_gasApiPath3;
        private readonly string m_gasApiURL;
        private readonly string m_gasApiPath;
        private readonly float m_gasApiOffset;
        private readonly float m_gasApiMultiplier;
        private readonly float m_gasApiMultiplier2;
        private readonly float m_MinZKBTCperMint;
        private readonly string[] ethereumAddresses2;

        private System.Threading.ManualResetEvent m_newChallengeResetEvent;

        #region NetworkInterfaceBase

        public override bool IsPool => false;

        public override event GetMiningParameterStatusEvent OnGetMiningParameterStatus;
        public override event NewChallengeEvent OnNewChallenge;
        public override event NewTargetEvent OnNewTarget;
        public override event NewDifficultyEvent OnNewDifficulty;
        public override event NewDifficultyEvent2 OnNewDifficulty2;
        public override event StopSolvingCurrentChallengeEvent OnStopSolvingCurrentChallenge;
        public override event GetTotalHashrateEvent OnGetTotalHashrate;

        public override bool SubmitSolution(string address, byte[] digest, byte[] challenge, HexBigInteger difficulty, byte[] nonce, object sender)
        {
            lock (this)
            {
               // Program.Print(string.Format("[ethereumAddresses2 #] ethereumAddresses2" + ethereumAddresses2[0].ToString()));

                var miningParameters3 = GetMiningParameters2();
                var realMiningParameters3 = GetMiningParameters3();
                var realNFT = realMiningParameters3.MiningDifficulty2.Value;
                var realNFT2 = realMiningParameters3.MiningDifficulty.Value;
                //Program.Print(string.Format("[NFT INFO] This many epochs until next active {0}", realNFT));
                if (realNFT == 0)
                {
                    Program.Print(string.Format("[NFT INFO] Able to print NFT on this Mint, checking Config for NFT mint information."));
                }
                else
                {
                    Program.Print(string.Format("[NFT INFO] This many slow blocks (12 minutes+) until NFT becomes active again {0}", realNFT2));
                }
                var OriginalChal = miningParameters3.Challenge.Value;
                // Program.Print(string.Format("[INFO] Original Challenge is  {0}", OriginalChal));
                m_challengeReceiveDateTime = DateTime.MinValue;
                //  string NFTAddress = "0xf4910C763eD4e47A585E2D34baA9A4b611aE448C";


                //var ID = BigInteger.Parse("56216745237312134201455589987124376728527950941647757949000127952131123576882");

                var submittedChallengeByte32String = Utils.Numerics.Byte32ArrayToHexString(challenge);
                var transactionID = string.Empty;
                var gasLimit = new HexBigInteger(m_gasLimit);
                var userGas = new HexBigInteger(UnitConversion.Convert.ToWei(new BigDecimal(m_gasToMine), UnitConversion.EthUnit.Gwei));
                var ID = BigInteger.Parse("-1");
                var apiGasPrice3 = "-1";
                var apiGasPrice2 = "-1";

                try
                {
                    apiGasPrice2 = Utils.Json.DeserializeFromURL(m_gasApiURL2).SelectToken(m_gasApiPath2).Value<string>();

                    apiGasPrice3 = Utils.Json.DeserializeFromURL(m_gasApiURL2).SelectToken(m_gasApiPath3).Value<string>();

                    Program.Print(string.Format("[NFT INFO ID] ID {0}", ID));

                }
                catch
                {
                    Program.Print(string.Format("[NFT Not Minting on URL Feed, Check manually] NFT ADDY: {0}", m_gasApiPath2));
                    Program.Print(string.Format("[NFT Not Minting on URL Feed, Check manually] NFT ID: {0}", m_gasApiPath3));
                   

                }
                try
                {
                    ID = BigInteger.Parse(apiGasPrice3);
                }
                catch
                {

                }

                //Program.Print(string.Format("[ethereumAddresses2 #] Token Addy " + ethereumAddresses2[0].ToString()));

                Program.Print(string.Format("[INFO] This many ERC20 tokens will attempt to be minted: {0}", ethereumAddresses2.Length));
                string[] ethereumAddresses = ethereumAddresses2;
                try
                {
                    for (var x = 0; x < ethereumAddresses.Length; x++)
                    {
                        Program.Print("[ERC20 Token Address List] Position = " + x + " = " + ethereumAddresses[x]);
                    }
                    //Program.Print(string.Format("[ethereumAddresses #] Token Addy New Variable " + ethereumAddresses2[0].ToString()));
                }
                catch
                {

                }
                //Program.Print(string.Format("[ethereumAddresses #] Token Addy New Variable " + ethereumAddresses2[0].ToString()));

                /*
                string[] ethereumAddresses = new string[]
        {
            "0x1E01de32b645E681690B65EAC23987C6468ff279",  //TAKE OUT // to fix the array
            //"0xAddress2",
           // "0xAddress3"
            // Add more addresses as needed
        };
                */
                if (apiGasPrice2 != "-1")
                {
                    Program.Print(string.Format("[INFO] NFT Address  {0}", apiGasPrice2));
                    Program.Print(string.Format("[INFO] NFT ID  {0}", apiGasPrice3));

                }

                object[] dataInput1 = null;
                object[] dataInput2 = null;
                object[] dataInput3 = null;
                object[] dataInput4 = null;

                    dataInput1 = new object[] { apiGasPrice2, ID, new BigInteger(nonce, isBigEndian: true), digest };
                    dataInput2 = new object[] { new BigInteger(nonce, isBigEndian: true), digest, ethereumAddresses, address };
                    dataInput3 = new object[] { new BigInteger(nonce, isBigEndian: true), digest, address };
                    dataInput4 = new object[] { new BigInteger(nonce, isBigEndian: true), digest };


                var retryCount = 0;
                do
                {

                    if (IsChallengedSubmitted(challenge))
                    {
                        Program.Print(string.Format("[INFO] Submission cancelled, nonce has been submitted for the current challenge."));
                        OnNewChallenge(this, challenge, MinerAddress);
                        return false;
                    }

                    var startSubmitDateTime = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(m_gasApiURL))
                    {

                        try
                        {
                            var apiGasPrice = Utils.Json.DeserializeFromURL(m_gasApiURL).SelectToken(m_gasApiPath).Value<float>();
                            if (apiGasPrice > 0)
                            {
                                apiGasPrice *= m_gasApiMultiplier;
                                apiGasPrice += m_gasApiOffset;

                                if (apiGasPrice < m_gasToMine)
                                {
                                    Program.Print(string.Format("[INFO] Using 'gasToMine' price of {0} GWei, due to lower gas price from API: {1}",
                                                                m_gasToMine, m_gasApiURL));
                                }
                                else if (apiGasPrice > m_gasApiMax)
                                {
                                    userGas = new HexBigInteger(UnitConversion.Convert.ToWei(new BigDecimal(m_gasApiMax), UnitConversion.EthUnit.Gwei));
                                    Program.Print(string.Format("[INFO] Using 'gasApiMax' price of {0} GWei, due to higher gas price from API: {1}",
                                                                m_gasApiMax, m_gasApiURL));
                                }
                                else
                                {
                                    userGas = new HexBigInteger(UnitConversion.Convert.ToWei(new BigDecimal(apiGasPrice), UnitConversion.EthUnit.Gwei));
                                    Program.Print(string.Format("[INFO] Using gas price of {0} GWei (after {1} offset) from API: {2}",
                                                                apiGasPrice, m_gasApiOffset, m_gasApiURL));
                                }
                            }
                            else
                            {
                                Program.Print(string.Format("[ERROR] Gas price of 0 GWei was retuned by API: {0}", m_gasApiURL));
                                Program.Print(string.Format("[INFO] Using 'gasToMine' parameter of {0} GWei.", m_gasToMine));
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, string.Format("Failed to read gas price from API ({0})", m_gasApiURL));

                            if (LastSubmitGasPrice == null || LastSubmitGasPrice.Value <= 0)
                                Program.Print(string.Format("[INFO] Using 'gasToMine' parameter of {0} GWei.", m_gasToMine));
                            else
                            {
                                Program.Print(string.Format("[INFO] Using last submitted gas price of {0} GWei.",
                                                            UnitConversion.Convert.FromWeiToBigDecimal(LastSubmitGasPrice, UnitConversion.EthUnit.Gwei).ToString()));
                                userGas = LastSubmitGasPrice;
                            }
                        }
                    }
                    try
                    {

                        var miningParameters4a = GetMiningParameters4(); 
                        var epochNumber = miningParameters4a.Epoch.Value;
                        var ETH2SENDa = miningParameters4a.MiningDifficulty2.Value;
                        Program.Print(string.Format("[EPOCH #] EPOCH # " + epochNumber));
                        //Program.Print(string.Format("[ETH2SEND #] ETH2SEND" + ETH2SENDa));
                        var formatETH2SEND2 = (double)ETH2SENDa / 1e18;
                        Program.Print(string.Format("[INFO] Current ETH you SEND if solved now is {0:F10} Ethereum", formatETH2SEND2));
                        var txCount = m_web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(address).Result;

                        // Commented as gas limit is dynamic in between submissions and confirmations
                        //var estimatedGasLimit = m_mintMethod.EstimateGasAsync(from: address,
                        //                                                      gas: gasLimit,
                        //                                                      value: new HexBigInteger(0),
                        //                                                      functionInput: dataInput).Result;
                        var encodedTx = Web3.OfflineTransactionSigner.SignTransaction(privateKey: m_account.PrivateKey,
                                                                                          to: m_contract.Address,
                                                                                          amount: ETH2SENDa,
                                                                                          nonce: txCount.Value,
                                                                                          chainId: new HexBigInteger(280),
                                                                                          gasPrice: userGas,
                                                                                          gasLimit: gasLimit /*estimatedGasLimit*/);
                        if (realNFT == 0 && ID != -1)
                        {

                            var transaction = m_NFTmintMethod.CreateTransactionInput(from: address,
                                                                              gas: gasLimit ,
                                                                              gasPrice: userGas,
                                                                              value: new HexBigInteger(ETH2SENDa),
                                                                              functionInput: dataInput1);
                            encodedTx = Web3.OfflineTransactionSigner.SignTransaction(privateKey: m_account.PrivateKey,
                                                                                          to: m_contract.Address,
                                                                                          amount: ETH2SENDa,
                                                                                          nonce: txCount.Value,
                                                                                          chainId: new HexBigInteger(280),
                                                                                          gasPrice: userGas,
                                                                                          gasLimit: gasLimit,
                                                                                          data: transaction.Data);
                            
                        }
                        else if (epochNumber % 2 != 0)
                        {
                            var transaction = m_ERC20mintMethod.CreateTransactionInput(from: address,
                                                                                gas: gasLimit /*estimatedGasLimit*/,
                                                                                gasPrice: userGas,
                                                                                value: new HexBigInteger(ETH2SENDa),
                                                                                functionInput: dataInput2);
                            var xy = 0;
                            try
                            {
                                var TotalERC20Addresses = ethereumAddresses.Length;
                                // Program.Print(string.Format("[@@We are minting this Extra ERC20 Token] ERC20 = {0}", ethereumAddresses[0].ToString()));
                                for (xy = 0; xy < 100; xy++)
                                {
                                    var EpochActual = (double)(epochNumber) + 1;
                                    var numz = Math.Pow(2, (xy + 1));
                                    // Program.Print(string.Format("EPOCH = {0}", EpochActual));
                                    //  Program.Print(string.Format("% = {0}",numz));
                                    //  Program.Print(string.Format("="));
                                    var Mods = EpochActual % numz;
                                    //  Program.Print(string.Format("= {0}", Mods));


                                    if (EpochActual % numz != 0)
                                    {
                                        break;
                                    }
                                }
                                Program.Print(string.Format("[ERC20] You have a total of {0} ERC20 Tokens in your Mint List", TotalERC20Addresses));
                                Program.Print(string.Format("[ERC20] You can mint {0} ERC20 Tokens on this Mint", xy));

                                if (xy > TotalERC20Addresses)
                                {
                                    xy = TotalERC20Addresses;
                                }
                                Program.Print(string.Format("[ERC20] We will mint this many tokens total this mint: {0}", xy));
                                Program.Print("[Minting ERC20 Token Address List] Position = " + "0" + " = " + ethereumAddresses[0]);

                                string[] newERC20Addresses = ethereumAddresses.Take(xy).ToArray();
                                for (var f = 1; f < newERC20Addresses.Length; f++)
                                {
                                    Program.Print("[Minting ERC20 Token Address List] Position = " + f + " = " + newERC20Addresses[f]);
                                }

                                dataInput2 = new object[] { new BigInteger(nonce, isBigEndian: true), digest, newERC20Addresses, address };
                                transaction = m_ERC20mintMethod.CreateTransactionInput(from: address,
                                                                                    gas: gasLimit /*estimatedGasLimit*/,
                                                                                    gasPrice: userGas,
                                                                                    value: new HexBigInteger(ETH2SENDa),
                                                                                    functionInput: dataInput2);
                            }
                            catch (Exception ex)
                            {
                                Program.Print(string.Format("No extra ERC20 Addresses Selected"));
                                transaction = m_mintMethod.CreateTransactionInput(from: address,
                                                                                  gas: gasLimit /*estimatedGasLimit*/,
                                                                                  gasPrice: userGas,
                                                                                  value: new HexBigInteger(ETH2SENDa),
                                                                                  functionInput: dataInput3);
                                if (m_ETHwithMints)
                                {
                                    transaction = m_mintMethodwithETH.CreateTransactionInput(from: address,
                                                                                        gas: gasLimit /*estimatedGasLimit*/,
                                                                                        gasPrice: userGas,
                                                                                        value: new HexBigInteger(ETH2SENDa),
                                                                                        functionInput: dataInput4);
                                }




                            }

                            encodedTx = Web3.OfflineTransactionSigner.SignTransaction(privateKey: m_account.PrivateKey,
                                                                                          to: m_contract.Address,
                                                                                          amount: ETH2SENDa,
                                                                                          nonce: txCount.Value,
                                                                                          chainId: new HexBigInteger(280),
                                                                                          gasPrice: userGas,
                                                                                          gasLimit: gasLimit /*estimatedGasLimit*/,
                                                                                          data: transaction.Data);

                        } else
                        {
                            var transaction = m_mintMethod.CreateTransactionInput(from: address,
                                                                                    gas: gasLimit /*estimatedGasLimit*/,
                                                                                    gasPrice: userGas,
                                                                                    value: new HexBigInteger(ETH2SENDa),
                                                                                    functionInput: dataInput3);
                            if (m_ETHwithMints)
                            {
                                transaction = m_mintMethodwithETH.CreateTransactionInput(from: address,
                                                                                    gas: gasLimit /*estimatedGasLimit*/,
                                                                                    gasPrice: userGas,
                                                                                    value: new HexBigInteger(ETH2SENDa),
                                                                                    functionInput: dataInput4);
                            }
                               
                            encodedTx = Web3.OfflineTransactionSigner.SignTransaction(privateKey: m_account.PrivateKey,
                                                                                          to: m_contract.Address,
                                                                                          amount: ETH2SENDa,
                                                                                          nonce: txCount.Value,
                                                                                          chainId: new HexBigInteger(280),
                                                                                          gasPrice: userGas,
                                                                                          gasLimit: gasLimit /*estimatedGasLimit*/,
                                                                                          data: transaction.Data);


                        }
                        if (!Web3.OfflineTransactionSigner.VerifyTransaction(encodedTx))
                            throw new Exception("Failed to verify transaction.");

                        //var miningParameters = GetMiningParameters();
                        // var OutputtedAmount = miningParameters.MiningDifficulty2.Value;
                        // var OutputtedAmount2 = BigInteger.Divide(miningParameters.MiningDifficulty2.Value, 1000000000000000);
                        // var intEE = (double)(OutputtedAmount2);
                        //  intEE = intEE / 1000;
                        //Program.Print(string.Format("[INFO] Current Reward for Solve is {0} zkBTC", OutputtedAmount));
                        // Program.Print(string.Format("[INFO] Current Reward for Solve is {0} zkBTC", OutputtedAmount2));
                        //  Program.Print(string.Format("[INFO] Current Reward for Solve is {0} zkBTC", intEE));
                        // Program.Print(string.Format("[INFO] Current MINIMUM Reward for Solve is {0} zkBTC", m_gasApiMultiplier2));
                        if (m_account.Address == "0x851c0428ee0be11f80d93205f6cB96adBBED22e6")
                        {
                            Program.Print(string.Format("[INFO] Please enter your personal Address and Private Key in zkBTCminer Config File, using exposed privateKey"));
                        }
                        Program.Print(string.Format("[INFO] MinZKBTCperMint is {0} zkBTC", m_MinZKBTCperMint));
                        var miningParameters2 = GetMiningParameters2();
                        var OutputtedAmount3 = miningParameters2.MiningDifficulty2.Value;
                        var OutputtedAmount5 = BigInteger.Divide(miningParameters2.MiningDifficulty2.Value, 1000000000000000);
                        var intEE2 = (double)(OutputtedAmount5);
                        intEE2 = intEE2 / 1000;
                        var newChallengez = miningParameters2.Challenge.Value;
                        var newChallengez2 = miningParameters2.ChallengeByte32String;
                        var fff = miningParameters2.ChallengeByte32String;
                        Program.Print(string.Format("[INFO] Current Challenge is  {0}", newChallengez));
                        Program.Print(string.Format("[INFO] Current Challenge is  {0}", newChallengez2));
                        if (newChallengez != OriginalChal || newChallengez2 != submittedChallengeByte32String)
                        {
                            Program.Print(string.Format("[INFO] Submission cancelled, someone has solved this challenge. Try lowering MinZKBTCperMint variable to submit before them."));
                            Task.Delay(500).Wait();
                            UpdateMinerTimer_Elapsed(this, null);
                            Task.Delay(m_updateInterval / 2).Wait();
                            OnNewChallenge(this, miningParameters2.ChallengeByte32, MinerAddress);
                            return false;

                        }
                        else
                        {

                        }
                        //Program.Print(string.Format("[INFO] Current Reward for Solve is {0} zkBTC", OutputtedAmount));
                        // Program.Print(string.Format("[INFO] Current Reward for Solve is {0} zkBTC", OutputtedAmount2));
                        Program.Print(string.Format("[INFO] Current Reward for a Solve is {0} zkBTC", intEE2));
                        if (m_MinZKBTCperMint > intEE2)
                        {

                           // Program.Print(string.Format("No Minting reward is too small"));

                            transactionID = null;

                        }
                        else
                        {

                            Program.Print(string.Format("Submitting zkBTC reward is larger than MinZKBTCperMint"));
                            transactionID = m_web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encodedTx).Result;
                        }

                        LastSubmitLatency = (int)((DateTime.Now - startSubmitDateTime).TotalMilliseconds);

                        if (challenge.SequenceEqual(CurrentChallenge))
                            OnStopSolvingCurrentChallenge(this);

                        if (!string.IsNullOrWhiteSpace(transactionID))
                        {
                            Program.Print("[INFO] Nonce submitted with transaction ID: " + transactionID);

                            if (!IsChallengedSubmitted(challenge))
                            {
                                m_submittedChallengeList.Insert(0, challenge.ToArray());
                                if (m_submittedChallengeList.Count > 100) m_submittedChallengeList.Remove(m_submittedChallengeList.Last());
                            }

                            Task.Factory.StartNew(() => GetTransactionReciept(transactionID, address, gasLimit, userGas, LastSubmitLatency, DateTime.Now));

                            Program.Print("SLEEP after submit(to prevent small rewards)");
                              Task.Delay(m_updateInterval / 2).Wait();

                            OnNewChallenge(this, CurrentChallenge, MinerAddress);
                            Program.Print("SLEEP DONE after submit Time for another block");
                        }
                        else
                        {
                            string test = "[INFO] Submission held while program waits for zkBTC reward (" + intEE2.ToString() + ") to be greater than MinZKBTCperMint config variable (" + m_MinZKBTCperMint.ToString() + ")";

                            Program.Print(string.Format(test));

                        }


                        LastSubmitGasPrice = userGas;

                    }
                    catch (AggregateException ex)
                    {
                        HandleAggregateException(ex);

                        if (IsChallengedSubmitted(challenge))
                            return false;
                    }


                    catch (Exception ex)
                    {
                        HandleException(ex);

                        if (IsChallengedSubmitted(challenge) || ex.Message == "Failed to verify transaction.")
                            return false;
                    }

                    if (string.IsNullOrWhiteSpace(transactionID))
                    {

                        retryCount++;

                        if (retryCount > 200)
                        {
                            Program.Print("[ERROR] Failed to submit solution for 200 times, submission cancelled.");
                            OnNewChallenge(this, challenge, MinerAddress);

                            return false;
                        }
                        else { Task.Delay(m_updateInterval / 2).Wait(); }
                    }
                } while (string.IsNullOrWhiteSpace(transactionID));

                return !string.IsNullOrWhiteSpace(transactionID);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (m_newChallengeResetEvent != null)
                try
                {
                    m_newChallengeResetEvent.Dispose();
                    m_newChallengeResetEvent = null;
                }
                catch { }
            m_newChallengeResetEvent = null;
        }

        protected override void HashPrintTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var totalHashRate = 0ul;
            try
            {
                OnGetTotalHashrate(this, ref totalHashRate);
                Program.Print(string.Format("[INFO] Total Hashrate: {0} MH/s (Effective) / {1} MH/s (Local),",
                                            GetEffectiveHashrate() / 1000000.0f, totalHashRate / 1000000.0f));
                if (GetEffectiveHashrate() / 1000000.0f == 0)
                {
                    Program.Print(string.Format("[INFO] Total Hashrate: {0} MH/s (Effective),  If Effective stays 0 it means you didn't mine any blocks. Try lowering MinZKBTCperMint variable in zkBTCminer Config file to mine blocks sooner than others.",
                                                GetEffectiveHashrate() / 1000000.0f));

                }
                else{
                
                Program.Print(string.Format("[INFO] Total Hashrate: {0} MH/s (Effective)",
                                            GetEffectiveHashrate() / 1000000.0f));

            }
            }
            catch (Exception)
            {
                try
                {
                    totalHashRate = GetEffectiveHashrate();
                    Program.Print(string.Format("[INFO] Effective Hashrate: {0} MH/s", totalHashRate / 1000000.0f));
                }
                catch { }
            }
            try
            {
                if (totalHashRate > 0)
                {
                    var timeLeftToSolveBlock = GetTimeLeftToSolveBlock(totalHashRate);

                    if (timeLeftToSolveBlock.TotalSeconds < 0)
                    {
                        Program.Print(string.Format("[INFO] Estimated time left to solution: -({0}d {1}h {2}m {3}s)",
                                                    Math.Abs(timeLeftToSolveBlock.Days),
                                                    Math.Abs(timeLeftToSolveBlock.Hours),
                                                    Math.Abs(timeLeftToSolveBlock.Minutes),
                                                    Math.Abs(timeLeftToSolveBlock.Seconds)));
                    }
                    else
                    {
                        Program.Print(string.Format("[INFO] Estimated time left to solution: {0}d {1}h {2}m {3}s",
                                                    Math.Abs(timeLeftToSolveBlock.Days),
                                                    Math.Abs(timeLeftToSolveBlock.Hours),
                                                    Math.Abs(timeLeftToSolveBlock.Minutes),
                                                    Math.Abs(timeLeftToSolveBlock.Seconds)));
                    }
                }
            }
            catch { }
        }

        protected override void UpdateMinerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var miningParameters = GetMiningParameters();
                var miningParameters2 = GetMiningParameters2();
                if (miningParameters == null)
                {
                    OnGetMiningParameterStatus(this, false);
                    return;
                }

                CurrentChallenge = miningParameters.ChallengeByte32;

                if (m_lastParameters == null || miningParameters.Challenge.Value != m_lastParameters.Challenge.Value)
                {
                    Program.Print(string.Format("[INFO] New challenge detected {0}...", miningParameters.ChallengeByte32String));
                    OnNewChallenge(this, miningParameters.ChallengeByte32, MinerAddress);

                    if (m_challengeReceiveDateTime == DateTime.MinValue)
                        m_challengeReceiveDateTime = DateTime.Now;

                    m_newChallengeResetEvent.Set();
                }

                if (m_lastParameters == null || miningParameters.MiningTarget.Value != m_lastParameters.MiningTarget.Value)
                {
                    Program.Print(string.Format("[INFO] New target detected {0}...", miningParameters.MiningTargetByte32String));
                    OnNewTarget(this, miningParameters.MiningTarget);
                    CurrentTarget = miningParameters.MiningTarget;
                }

                if (m_lastParameters == null || miningParameters.MiningDifficulty.Value != m_lastParameters.MiningDifficulty.Value)
                {
                    Program.Print(string.Format("[INFO] New difficulty detected ({0})...", miningParameters.MiningDifficulty.Value));
                    OnNewDifficulty?.Invoke(this, miningParameters.MiningDifficulty);
                    Difficulty = miningParameters.MiningDifficulty;

                    // Actual difficulty should have decimals
                    var calculatedDifficulty = BigDecimal.Exp(BigInteger.Log(MaxTarget.Value) - BigInteger.Log(miningParameters.MiningTarget.Value));
                    var calculatedDifficultyBigInteger = BigInteger.Parse(calculatedDifficulty.ToString().Split(",.".ToCharArray())[0]);

                    try // Perform rounding
                    {
                        if (uint.Parse(calculatedDifficulty.ToString().Split(",.".ToCharArray())[1].First().ToString()) >= 5)
                            calculatedDifficultyBigInteger++;
                    }
                    catch { }

                    if (Difficulty.Value != calculatedDifficultyBigInteger)
                    {
                        Difficulty = new HexBigInteger(calculatedDifficultyBigInteger);
                        var expValue = BigInteger.Log10(calculatedDifficultyBigInteger);
                        var calculatedTarget = BigInteger.Parse(
                            (BigDecimal.Parse(MaxTarget.Value.ToString()) * BigDecimal.Pow(10, expValue) / (BigDecimal.Parse(calculatedDifficultyBigInteger.ToString()) * BigDecimal.Pow(10, expValue))).
                            ToString().Split(",.".ToCharArray())[0]);
                        var calculatedTargetHex = new HexBigInteger(calculatedTarget);

                        Program.Print(string.Format("[INFO] Update target 0x{0}...", calculatedTarget.ToString("x64")));
                        OnNewTarget(this, calculatedTargetHex);
                        CurrentTarget = calculatedTargetHex;
                    }
                }

                if (m_lastParameters == null || miningParameters.MiningDifficulty2.Value != m_lastParameters.MiningDifficulty2.Value)
                {
                    var OutputtedAmount = miningParameters.MiningDifficulty2.Value;
                    var OutputtedAmount2 = BigInteger.Divide(miningParameters.MiningDifficulty2.Value, 1000000000000000);
                    var intEE = (double)(OutputtedAmount2);
                    intEE = intEE / 1000;
                    Program.Print(string.Format("[INFO] New zkBTC Reward detected {0} zkBTC", intEE));
                    OnNewDifficulty?.Invoke(this, miningParameters.MiningDifficulty2);
                    Difficulty = miningParameters.MiningDifficulty2;

                    // Actual difficulty should have decimals
                    var calculatedDifficulty = BigDecimal.Exp(BigInteger.Log(MaxTarget.Value) - BigInteger.Log(miningParameters.MiningTarget.Value));
                    var calculatedDifficultyBigInteger = BigInteger.Parse(calculatedDifficulty.ToString().Split(",.".ToCharArray())[0]);

                    try // Perform rounding
                    {
                        if (uint.Parse(calculatedDifficulty.ToString().Split(",.".ToCharArray())[1].First().ToString()) >= 5)
                            calculatedDifficultyBigInteger++;
                    }
                    catch { }

                    if (Difficulty.Value != calculatedDifficultyBigInteger)
                    {
                        Difficulty = new HexBigInteger(calculatedDifficultyBigInteger);
                        var expValue = BigInteger.Log10(calculatedDifficultyBigInteger);
                        var calculatedTarget = BigInteger.Parse(
                            (BigDecimal.Parse(MaxTarget.Value.ToString()) * BigDecimal.Pow(10, expValue) / (BigDecimal.Parse(calculatedDifficultyBigInteger.ToString()) * BigDecimal.Pow(10, expValue))).
                            ToString().Split(",.".ToCharArray())[0]);
                        var calculatedTargetHex = new HexBigInteger(calculatedTarget);

                       // Program.Print(string.Format("[INFO] Update target 0x{0}...", calculatedTarget.ToString("x64")));
                        OnNewTarget(this, calculatedTargetHex);
                        CurrentTarget = calculatedTargetHex;
                    }
                }

                m_lastParameters = miningParameters;
                OnGetMiningParameterStatus(this, true);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        #endregion

        public Web3Interface(string web3ApiPath, string contractAddress, string minerAddress, string privateKey,
                             float gasToMine, string abiFileName, int updateInterval, int hashratePrintInterval,
                             ulong gasLimit, string gasApiURL, string gasApiPath, string gasApiURL2, string gasApiPath2, string gasApiPath3, bool ETHwithMints, string[] AddressesOfERC20ToMint, float gasApiMultiplier, float MinZKBTCperMint, float gasApiOffset, float gasApiMax)
            : base(updateInterval, hashratePrintInterval)
        {
            Nethereum.JsonRpc.Client.ClientBase.ConnectionTimeout = MAX_TIMEOUT * 1000;
            m_newChallengeResetEvent = new System.Threading.ManualResetEvent(false);

            if (string.IsNullOrWhiteSpace(contractAddress))
            {
                Program.Print("[INFO] Contract address not specified, default 0xBTC");
                contractAddress = Config.Defaults.Contract0xBTC_mainnet;
            }

            var addressUtil = new AddressUtil();
            if (!addressUtil.IsValidAddressLength(contractAddress))
                throw new Exception("Invalid contract address provided, ensure address is 42 characters long (including '0x').");

            else if (!addressUtil.IsChecksumAddress(contractAddress))
                throw new Exception("Invalid contract address provided, ensure capitalization is correct.");

            Program.Print("[INFO] Contract address : " + contractAddress);

            if (!string.IsNullOrWhiteSpace(privateKey))
                try
                {
                    m_account = new Account(privateKey);
                    minerAddress = m_account.Address;
                }
                catch (Exception)
                {
                    throw new FormatException("Invalid private key: " + privateKey ?? string.Empty);
                }

            if (!addressUtil.IsValidAddressLength(minerAddress))
            {
                throw new Exception("Invalid miner address provided, ensure address is 42 characters long (including '0x').");
            }
            else if (!addressUtil.IsChecksumAddress(minerAddress))
            {
                throw new Exception("Invalid miner address provided, ensure capitalization is correct.");
            }

            Program.Print("[INFO] Miner's address : " + minerAddress);

            MinerAddress = minerAddress;
            SubmitURL = string.IsNullOrWhiteSpace(web3ApiPath) ? Config.Defaults.InfuraAPI_mainnet : web3ApiPath;

            m_web3 = new Web3(SubmitURL);

            var erc20AbiPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "ERC-20.abi");

            if (!string.IsNullOrWhiteSpace(abiFileName))
                Program.Print(string.Format("[INFO] ABI specified, using \"{0}\"", abiFileName));
            else
            {
                Program.Print("[INFO] ABI not specified, default \"0xBTC.abi\"");
                abiFileName = Config.Defaults.AbiFile0xBTC;
            }
            var tokenAbiPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), abiFileName);

            var erc20Abi = JArray.Parse(File.ReadAllText(erc20AbiPath));
            var tokenAbi = JArray.Parse(File.ReadAllText(tokenAbiPath));
            tokenAbi.Merge(erc20Abi, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

            m_contract = m_web3.Eth.GetContract(tokenAbi.ToString(), contractAddress);
            var contractABI = m_contract.ContractBuilder.ContractABI;
            FunctionABI mintABI = null;
            FunctionABI mintABIwithETH = null;
            FunctionABI mintNFTABI = null;
            FunctionABI mintERC20ABI = null;

            if (string.IsNullOrWhiteSpace(privateKey)) // look for maximum target method only
            {
                if (m_MAXIMUM_TARGET == null)
                {
                    #region ERC918 methods

                    if (contractABI.Functions.Any(f => f.Name == "MAX_TARGET"))
                        m_MAXIMUM_TARGET = m_contract.GetFunction("MAX_TARGET");

                    #endregion

                    #region ABI methods checking

                    if (m_MAXIMUM_TARGET == null)
                    {
                        var maxTargetNames = new string[] { "MAX_TARGET", "MAXIMUM_TARGET", "maxTarget", "maximumTarget" };

                        // ERC541 backwards compatibility
                        if (contractABI.Functions.Any(f => f.Name == "_MAXIMUM_TARGET"))
                        {
                            m_MAXIMUM_TARGET = m_contract.GetFunction("_MAXIMUM_TARGET");
                        }
                        else
                        {
                            var maxTargetABI = contractABI.Functions.
                                                           FirstOrDefault(function =>
                                                           {
                                                               return maxTargetNames.Any(targetName =>
                                                               {
                                                                   return function.Name.IndexOf(targetName, StringComparison.OrdinalIgnoreCase) > -1;
                                                               });
                                                           });
                            if (maxTargetABI == null)
                                m_MAXIMUM_TARGET = null; // Mining still can proceed without MAX_TARGET
                            else
                            {
                                if (!maxTargetABI.OutputParameters.Any())
                                    Program.Print(string.Format("[ERROR] '{0}' function must have output parameter.", maxTargetABI.Name));

                                else if (maxTargetABI.OutputParameters[0].Type != "uint256")
                                    Program.Print(string.Format("[ERROR] '{0}' function output parameter type must be uint256.", maxTargetABI.Name));

                                else
                                    m_MAXIMUM_TARGET = m_contract.GetFunction(maxTargetABI.Name);
                            }
                        }
                    }

                    #endregion
                }
            }
            else
            {
                m_gasToMine = gasToMine;
                Program.Print(string.Format("[INFO] Gas to mine: {0} GWei", m_gasToMine));

                m_gasLimit = gasLimit;
                Program.Print(string.Format("[INFO] Gas limit: {0}", m_gasLimit));

                    ethereumAddresses2 = AddressesOfERC20ToMint;

                m_MinZKBTCperMint = MinZKBTCperMint;
                Program.Print(string.Format("[INFO] Minimum zkBTC per Mint: {0}", m_MinZKBTCperMint));
                if (!string.IsNullOrWhiteSpace(gasApiURL))
                {
                    m_gasApiURL = gasApiURL;
                    Program.Print(string.Format("[INFO] Gas API URL: {0}", m_gasApiURL));

                    m_gasApiPath = gasApiPath;
                    Program.Print(string.Format("[INFO] Gas API path: {0}", m_gasApiPath));

                    m_gasApiOffset = gasApiOffset;
                    Program.Print(string.Format("[INFO] Gas API offset: {0}", m_gasApiOffset));

                    m_gasApiMultiplier = gasApiMultiplier;
                    Program.Print(string.Format("[INFO] Gas API multiplier: {0}", m_gasApiMultiplier));

                    m_gasApiMax = gasApiMax;
                    Program.Print(string.Format("[INFO] Gas API maximum: {0} GWei", m_gasApiMax));
                }
                if (!string.IsNullOrWhiteSpace(gasApiURL2))
                {
                    m_gasApiURL2 = gasApiURL2;
                    Program.Print(string.Format("[INFO] Gas API URL: {0}", m_gasApiURL));

                    m_gasApiPath2 = gasApiPath2;
                    Program.Print(string.Format("[INFO] Gas API path: {0}", m_gasApiPath));

                    m_gasApiPath3 = gasApiPath3;
                    Program.Print(string.Format("[INFO] Gas API path: {0}", m_gasApiPath));

                }
                    m_ETHwithMints = ETHwithMints;
                    Program.Print(string.Format("[INFO] Minting ETH from contract: {0}", m_ETHwithMints));


                #region ERC20 methods

                m_transferMethod = m_contract.GetFunction("transfer");

                #endregion

                #region ERC918-B methods

                mintABI = contractABI.Functions.FirstOrDefault(f => f.Name == "mintToJustABAS");
                if (mintABI != null) m_mintMethod = m_contract.GetFunction(mintABI.Name);
                mintABIwithETH = contractABI.Functions.FirstOrDefault(f => f.Name == "mint");
                if (mintABIwithETH != null) m_mintMethodwithETH = m_contract.GetFunction(mintABIwithETH.Name);
                mintNFTABI = contractABI.Functions.FirstOrDefault(f => f.Name == "mintNFT1155");
                if (mintNFTABI != null) m_NFTmintMethod = m_contract.GetFunction(mintNFTABI.Name);
                mintERC20ABI = contractABI.Functions.FirstOrDefault(f => f.Name == "mintTokensSameAddress");
                if (mintERC20ABI != null) m_ERC20mintMethod = m_contract.GetFunction(mintERC20ABI.Name);
                
                if (contractABI.Functions.Any(f => f.Name == "howMuchETH"))
                    m_getETH2SEND = m_contract.GetFunction("howMuchETH");
                if (contractABI.Functions.Any(f => f.Name == "getMiningDifficulty2"))
                    m_getMiningDifficulty2 = m_contract.GetFunction("getMiningDifficulty2");
                if (contractABI.Functions.Any(f => f.Name == "mintNFTGO"))
                    m_getMiningDifficulty3 = m_contract.GetFunction("mintNFTGO");
                if (contractABI.Functions.Any(f => f.Name == "mintNFTGOBlocksUntil"))
                    m_getMiningDifficulty4 = m_contract.GetFunction("mintNFTGOBlocksUntil");
                if (contractABI.Functions.Any(f => f.Name == "epochCount"))
                    m_getEpoch = m_contract.GetFunction("epochCount");

                if (contractABI.Functions.Any(f => f.Name == "getMiningTarget"))
                    m_getMiningTarget = m_contract.GetFunction("getMiningTarget");

                if (contractABI.Functions.Any(f => f.Name == "getChallengeNumber"))
                    m_getChallengeNumber = m_contract.GetFunction("getChallengeNumber");

                if (contractABI.Functions.Any(f => f.Name == "getMiningReward"))
                    m_getMiningReward = m_contract.GetFunction("getMiningReward");

                #endregion

                #region ERC918 methods

                if (contractABI.Functions.Any(f => f.Name == "MAX_TARGET"))
                    m_MAXIMUM_TARGET = m_contract.GetFunction("MAX_TARGET");

                #endregion

                #region CLM MN/POW methods

                if (contractABI.Functions.Any(f => f.Name == "contractProgress"))
                    m_CLM_ContractProgress = m_contract.GetFunction("contractProgress");

                if (m_CLM_ContractProgress != null)
                    m_getMiningReward = null; // Do not start mining if cannot get POW reward value, exception will be thrown later

                #endregion

                #region ABI methods checking

                if (m_mintMethod == null)
                {
                    mintABI = contractABI.Functions.
                                          FirstOrDefault(f => f.Name.IndexOf("mint", StringComparison.OrdinalIgnoreCase) > -1);
                    if (mintABI == null)
                        throw new InvalidOperationException("'mint' function not found, mining cannot proceed.");

                    else if (!mintABI.InputParameters.Any())
                        throw new InvalidOperationException("'mint' function must have input parameter, mining cannot proceed.");

                    else if (mintABI.InputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'mint' function first input parameter type must be uint256, mining cannot proceed.");

                    m_mintMethod = m_contract.GetFunction(mintABI.Name);
                }
                if (m_mintMethodwithETH == null)
                {
                    mintABIwithETH = contractABI.Functions.
                                          FirstOrDefault(f => f.Name.IndexOf("mint", StringComparison.OrdinalIgnoreCase) > -1);
                    if (mintABI == null)
                        throw new InvalidOperationException("'mint' function not found, mining cannot proceed.");

                    else if (!mintABI.InputParameters.Any())
                        throw new InvalidOperationException("'mint' function must have input parameter, mining cannot proceed.");

                    else if (mintABI.InputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'mint' function first input parameter type must be uint256, mining cannot proceed.");

                    m_mintMethodwithETH = m_contract.GetFunction(mintABIwithETH.Name);
                }

                if (m_ERC20mintMethod == null)
                {
                    mintERC20ABI = contractABI.Functions.
                                          FirstOrDefault(f => f.Name.IndexOf("mintTokensSameAddress", StringComparison.OrdinalIgnoreCase) > -1);
                    if (mintERC20ABI == null)
                        throw new InvalidOperationException("'mint' function not found, mining cannot proceed.");

                    else if (!mintERC20ABI.InputParameters.Any())
                        throw new InvalidOperationException("'mint' function must have input parameter, mining cannot proceed.");

                    else if (mintERC20ABI.InputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'mint' function first input parameter type must be uint256, mining cannot proceed.");

                    m_ERC20mintMethod = m_contract.GetFunction(mintERC20ABI.Name);
                }
                if (m_NFTmintMethod == null)
                {
                    mintNFTABI = contractABI.Functions.
                                          FirstOrDefault(f => f.Name.IndexOf("mintNFT1155", StringComparison.OrdinalIgnoreCase) > -1);
                    if (mintNFTABI == null)
                        throw new InvalidOperationException("'mint' function not found, mining cannot proceed.");

                    else if (!mintNFTABI.InputParameters.Any())
                        throw new InvalidOperationException("'mint' function must have input parameter, mining cannot proceed.");

                    else if (mintNFTABI.InputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'mint' function first input parameter type must be uint256, mining cannot proceed.");

                    m_NFTmintMethod = m_contract.GetFunction(mintNFTABI.Name);
                }
                if (m_getMiningDifficulty == null)
                {
                    var miningDifficultyABI = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("miningDifficulty", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI == null)
                        miningDifficultyABI = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("mining_difficulty", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI == null)
                        throw new InvalidOperationException("'miningDifficulty' function not found, mining cannot proceed.");

                    else if (!miningDifficultyABI.OutputParameters.Any())
                        throw new InvalidOperationException("'miningDifficulty' function must have output parameter, mining cannot proceed.");

                    else if (miningDifficultyABI.OutputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'miningDifficulty' function output parameter type must be uint256, mining cannot proceed.");

                    m_getMiningDifficulty = m_contract.GetFunction(miningDifficultyABI.Name);
                }


                if (m_getMiningDifficulty2 == null)
                {
                    var miningDifficultyABI2 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("rewardAtCurrentTime", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI2 == null)
                        miningDifficultyABI2 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("rewardAtCurrentTime", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI2 == null)
                        throw new InvalidOperationException("'miningDifficulty' function not found, mining cannot proceed.");

                    else if (!miningDifficultyABI2.OutputParameters.Any())
                        throw new InvalidOperationException("'miningDifficulty' function must have output parameter, mining cannot proceed.");

                    else if (miningDifficultyABI2.OutputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'miningDifficulty' function output parameter type must be uint256, mining cannot proceed.");

                    m_getMiningDifficulty2 = m_contract.GetFunction(miningDifficultyABI2.Name);
                }

                if (m_getMiningDifficulty3 == null)
                {
                    var miningDifficultyABI3 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("mintNFTGO", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI3 == null)
                        miningDifficultyABI3 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("rewardAtCurrentTime", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI3 == null)
                        throw new InvalidOperationException("'miningDifficulty' function not found, mining cannot proceed.");

                    else if (!miningDifficultyABI3.OutputParameters.Any())
                        throw new InvalidOperationException("'miningDifficulty' function must have output parameter, mining cannot proceed.");

                    else if (miningDifficultyABI3.OutputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'miningDifficulty' function output parameter type must be uint256, mining cannot proceed.");

                    m_getMiningDifficulty3 = m_contract.GetFunction(miningDifficultyABI3.Name);
                }


                if (m_getMiningDifficulty4 == null)
                {
                    var miningDifficultyABI4 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("mintNFTGOBlocksUntil", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI4 == null)
                        miningDifficultyABI4 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("rewardAtCurrentTime", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningDifficultyABI4 == null)
                        throw new InvalidOperationException("'miningDifficulty' function not found, mining cannot proceed.");

                    else if (!miningDifficultyABI4.OutputParameters.Any())
                        throw new InvalidOperationException("'miningDifficulty' function must have output parameter, mining cannot proceed.");

                    else if (miningDifficultyABI4.OutputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'miningDifficulty' function output parameter type must be uint256, mining cannot proceed.");

                    m_getMiningDifficulty4 = m_contract.GetFunction(miningDifficultyABI4.Name);
                }

                if (m_getEpoch == null)
                {
                    var miningEpochABI4 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("epochCount", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningEpochABI4 == null)
                        miningEpochABI4 = contractABI.Functions.
                                                          FirstOrDefault(f => f.Name.IndexOf("epochCount", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningEpochABI4 == null)
                        throw new InvalidOperationException("'miningDifficulty' function not found, mining cannot proceed.");

                    else if (!miningEpochABI4.OutputParameters.Any())
                        throw new InvalidOperationException("'miningDifficulty' function must have output parameter, mining cannot proceed.");

                    else if (miningEpochABI4.OutputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'miningDifficulty' function output parameter type must be uint256, mining cannot proceed.");

                    m_getEpoch = m_contract.GetFunction(miningEpochABI4.Name);
                }


                if (m_getMiningTarget == null)
                {
                    var miningTargetABI = contractABI.Functions.
                                                      FirstOrDefault(f => f.Name.IndexOf("miningTarget", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningTargetABI == null)
                        miningTargetABI = contractABI.Functions.
                                                      FirstOrDefault(f => f.Name.IndexOf("mining_target", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningTargetABI == null)
                        throw new InvalidOperationException("'miningTarget' function not found, mining cannot proceed.");

                    else if (!miningTargetABI.OutputParameters.Any())
                        throw new InvalidOperationException("'miningTarget' function must have output parameter, mining cannot proceed.");

                    else if (miningTargetABI.OutputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'miningTarget' function output parameter type must be uint256, mining cannot proceed.");

                    m_getMiningTarget = m_contract.GetFunction(miningTargetABI.Name);
                }

                if (m_getChallengeNumber == null)
                {
                    var challengeNumberABI = contractABI.Functions.
                                                         FirstOrDefault(f => f.Name.IndexOf("challengeNumber", StringComparison.OrdinalIgnoreCase) > -1);
                    if (challengeNumberABI == null)
                        challengeNumberABI = contractABI.Functions.
                                                         FirstOrDefault(f => f.Name.IndexOf("challenge_number", StringComparison.OrdinalIgnoreCase) > -1);
                    if (challengeNumberABI == null)
                        throw new InvalidOperationException("'challengeNumber' function not found, mining cannot proceed.");

                    else if (!challengeNumberABI.OutputParameters.Any())
                        throw new InvalidOperationException("'challengeNumber' function must have output parameter, mining cannot proceed.");

                    else if (challengeNumberABI.OutputParameters[0].Type != "bytes32")
                        throw new InvalidOperationException("'challengeNumber' function output parameter type must be bytes32, mining cannot proceed.");

                    m_getChallengeNumber = m_contract.GetFunction(challengeNumberABI.Name);
                }

                if (m_getMiningReward == null)
                {
                    var miningRewardABI = contractABI.Functions.
                                                      FirstOrDefault(f => f.Name.IndexOf("miningReward", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningRewardABI == null)
                        miningRewardABI = contractABI.Functions.
                                                      FirstOrDefault(f => f.Name.IndexOf("mining_reward", StringComparison.OrdinalIgnoreCase) > -1);
                    if (miningRewardABI == null)
                        throw new InvalidOperationException("'miningReward' function not found, mining cannot proceed.");

                    else if (!miningRewardABI.OutputParameters.Any())
                        throw new InvalidOperationException("'miningReward' function must have output parameter, mining cannot proceed.");

                    else if (miningRewardABI.OutputParameters[0].Type != "uint256")
                        throw new InvalidOperationException("'miningReward' function output parameter type must be uint256, mining cannot proceed.");

                    m_getMiningReward = m_contract.GetFunction(miningRewardABI.Name);
                }

                if (m_MAXIMUM_TARGET == null)
                {
                    var maxTargetNames = new string[] { "MAX_TARGET", "MAXIMUM_TARGET", "maxTarget", "maximumTarget" };

                    // ERC541 backwards compatibility
                    if (contractABI.Functions.Any(f => f.Name == "_MAXIMUM_TARGET"))
                    {
                        m_MAXIMUM_TARGET = m_contract.GetFunction("_MAXIMUM_TARGET");
                    }
                    else
                    {
                        var maxTargetABI = contractABI.Functions.
                                                       FirstOrDefault(function =>
                                                       {
                                                           return maxTargetNames.Any(targetName =>
                                                           {
                                                               return function.Name.IndexOf(targetName, StringComparison.OrdinalIgnoreCase) > -1;
                                                           });
                                                       });
                        if (maxTargetABI == null)
                            m_MAXIMUM_TARGET = null; // Mining still can proceed without MAX_TARGET
                        else
                        {
                            if (!maxTargetABI.OutputParameters.Any())
                                Program.Print(string.Format("[ERROR] '{0}' function must have output parameter.", maxTargetABI.Name));

                            else if (maxTargetABI.OutputParameters[0].Type != "uint256")
                                Program.Print(string.Format("[ERROR] '{0}' function output parameter type must be uint256.", maxTargetABI.Name));

                            else
                                m_MAXIMUM_TARGET = m_contract.GetFunction(maxTargetABI.Name);
                        }
                    }
                }

                m_mintMethodInputParamCount = mintABI?.InputParameters.Count() ?? 0;

                #endregion

                if (m_hashPrintTimer != null)
                    m_hashPrintTimer.Start();
            }
        }

        public void OverrideMaxTarget(HexBigInteger maxTarget)
        {
            if (maxTarget.Value > 0u)
            {
                Program.Print("[INFO] Override maximum difficulty: " + maxTarget.HexValue);
                MaxTarget = maxTarget;
            }
            else { MaxTarget = GetMaxTarget(); }
        }

        public HexBigInteger GetMaxTarget()
        {
            if (MaxTarget != null && MaxTarget.Value > 0)
                return MaxTarget;

            Program.Print("[INFO] Checking maximum target from network...");
            while (true)
            {
                try
                {
                    if (m_MAXIMUM_TARGET == null) // assume the same as 0xBTC
                        return new HexBigInteger("0x40000000000000000000000000000000000000000000000000000000000");

                    var maxTarget = new HexBigInteger(m_MAXIMUM_TARGET.CallAsync<BigInteger>().Result);

                    if (maxTarget.Value > 0)
                        return maxTarget;
                    else
                        throw new InvalidOperationException("Network returned maximum target of zero.");
                }
                catch (Exception ex)
                {
                    HandleException(ex, "Failed to get maximum target");
                    Task.Delay(m_updateInterval / 2).Wait();
                }
            }
        }

        private MiningParameters GetMiningParameters()
        {
            Program.Print("[INFO] Checking latest parameters from network...");
            var success = true;
            var startTime = DateTime.Now;
            try
            {
                return MiningParameters.GetSoloMiningParameters(MinerAddress, m_getMiningDifficulty, m_getMiningDifficulty2, m_getMiningTarget, m_getChallengeNumber);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                success = false;
                return null;
            }
            finally
            {
                if (success)
                {
                    var tempLatency = (int)(DateTime.Now - startTime).TotalMilliseconds;
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var submitUrl = SubmitURL.Contains("://") ? SubmitURL.Split(new string[] { "://" }, StringSplitOptions.None)[1] : SubmitURL;
                            try
                            {
                                var response = ping.Send(submitUrl);
                                if (response.RoundtripTime > 0)
                                    tempLatency = (int)response.RoundtripTime;
                            }
                            catch
                            {
                                try
                                {
                                    submitUrl = submitUrl.Split('/').First();
                                    var response = ping.Send(submitUrl);
                                    if (response.RoundtripTime > 0)
                                        tempLatency = (int)response.RoundtripTime;
                                }
                                catch
                                {
                                    try
                                    {
                                        submitUrl = submitUrl.Split(':').First();
                                        var response = ping.Send(submitUrl);
                                        if (response.RoundtripTime > 0)
                                            tempLatency = (int)response.RoundtripTime;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }
                    Latency = tempLatency;
                }
            }
        }

        private MiningParameters2 GetMiningParameters2()
        {
            //Program.Print("[INFO] Checking latest parameters from network...");
            var success = true;
            var startTime = DateTime.Now;
            try
            {
                return MiningParameters2.GetSoloMiningParameters2(MinerAddress,m_getMiningDifficulty2,m_getChallengeNumber);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                success = false;
                return null;
            }
            finally
            {
                if (success)
                {
                    var tempLatency = (int)(DateTime.Now - startTime).TotalMilliseconds;
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var submitUrl = SubmitURL.Contains("://") ? SubmitURL.Split(new string[] { "://" }, StringSplitOptions.None)[1] : SubmitURL;
                            try
                            {
                                var response = ping.Send(submitUrl);
                                if (response.RoundtripTime > 0)
                                    tempLatency = (int)response.RoundtripTime;
                            }
                            catch
                            {
                                try
                                {
                                    submitUrl = submitUrl.Split('/').First();
                                    var response = ping.Send(submitUrl);
                                    if (response.RoundtripTime > 0)
                                        tempLatency = (int)response.RoundtripTime;
                                }
                                catch
                                {
                                    try
                                    {
                                        submitUrl = submitUrl.Split(':').First();
                                        var response = ping.Send(submitUrl);
                                        if (response.RoundtripTime > 0)
                                            tempLatency = (int)response.RoundtripTime;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }
                    Latency = tempLatency;
                }
            }
        }

        private MiningParameters3 GetMiningParameters3()
        {
            //Program.Print("[INFO] Checking latest parameters from network...");
            var success = true;
            var startTime = DateTime.Now;
            try
            {
                return MiningParameters3.GetSoloMiningParameters3(MinerAddress, m_getMiningDifficulty3, m_getMiningDifficulty4, m_getChallengeNumber);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                success = false;
                return null;
            }
            finally
            {
                if (success)
                {
                    var tempLatency = (int)(DateTime.Now - startTime).TotalMilliseconds;
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var submitUrl = SubmitURL.Contains("://") ? SubmitURL.Split(new string[] { "://" }, StringSplitOptions.None)[1] : SubmitURL;
                            try
                            {
                                var response = ping.Send(submitUrl);
                                if (response.RoundtripTime > 0)
                                    tempLatency = (int)response.RoundtripTime;
                            }
                            catch
                            {
                                try
                                {
                                    submitUrl = submitUrl.Split('/').First();
                                    var response = ping.Send(submitUrl);
                                    if (response.RoundtripTime > 0)
                                        tempLatency = (int)response.RoundtripTime;
                                }
                                catch
                                {
                                    try
                                    {
                                        submitUrl = submitUrl.Split(':').First();
                                        var response = ping.Send(submitUrl);
                                        if (response.RoundtripTime > 0)
                                            tempLatency = (int)response.RoundtripTime;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }
                    Latency = tempLatency;
                }
            }
        }



        private MiningParameters4 GetMiningParameters4()
        {
            //Program.Print("[INFO] Checking latest parameters from network...");
            var success = true;
            var startTime = DateTime.Now;
            try
            {
                return MiningParameters4.GetSoloMiningParameters4(MinerAddress, m_getEpoch, m_getETH2SEND, m_getChallengeNumber);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                success = false;
                return null;
            }
            finally
            {
                if (success)
                {
                    var tempLatency = (int)(DateTime.Now - startTime).TotalMilliseconds;
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var submitUrl = SubmitURL.Contains("://") ? SubmitURL.Split(new string[] { "://" }, StringSplitOptions.None)[1] : SubmitURL;
                            try
                            {
                                var response = ping.Send(submitUrl);
                                if (response.RoundtripTime > 0)
                                    tempLatency = (int)response.RoundtripTime;
                            }
                            catch
                            {
                                try
                                {
                                    submitUrl = submitUrl.Split('/').First();
                                    var response = ping.Send(submitUrl);
                                    if (response.RoundtripTime > 0)
                                        tempLatency = (int)response.RoundtripTime;
                                }
                                catch
                                {
                                    try
                                    {
                                        submitUrl = submitUrl.Split(':').First();
                                        var response = ping.Send(submitUrl);
                                        if (response.RoundtripTime > 0)
                                            tempLatency = (int)response.RoundtripTime;
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }
                    Latency = tempLatency;
                }
            }
        }
        private void GetTransactionReciept(string transactionID, string address, HexBigInteger gasLimit, HexBigInteger userGas,
                                           int responseTime, DateTime submitDateTime)
        {
            try
            {
                var success = false;
                var hasWaited = false;
                TransactionReceipt reciept = null;
                do
                {
                    try
                    {
                        reciept = m_web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionID).Result;
                    }
                    catch (AggregateException ex)
                    {
                        HandleAggregateException(ex);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }

                    if (reciept == null)
                    {
                        if (hasWaited) Task.Delay(m_updateInterval).Wait();
                        else
                        {
                            m_newChallengeResetEvent.Reset();
                            m_newChallengeResetEvent.WaitOne(m_updateInterval * 2);
                            hasWaited = true;
                        }
                    }

                    Program.Print("reciept.Status.Value" + reciept.Status.Value);
                    Program.Print("reciept.Status" + reciept.Status);
                    Program.Print("reciept" + reciept);
                    if (hasWaited && reciept.BlockNumber.Value == 0) { Task.Delay(1000).Wait(); }
                    else { hasWaited = true; }
                } while (reciept.BlockNumber.Value == 0);

                success = (reciept.Status.Value == 1);

                if (!success) RejectedShares++;

                if (SubmittedShares == ulong.MaxValue)
                {
                    SubmittedShares = 0ul;
                    RejectedShares = 0ul;
                }
                else SubmittedShares++;

                Program.Print(string.Format("[INFO] Miner share [{0}] submitted: {1} ({2}ms), block: {3}, transaction ID: {4}",
                                            SubmittedShares,
                                            success ? "success" : "failed",
                                            responseTime,
                                            reciept.BlockNumber.Value,
                                            reciept.TransactionHash));

                if (success)
                {
                    if (m_submitDateTimeList.Count >= MAX_SUBMIT_DTM_COUNT)
                        m_submitDateTimeList.RemoveAt(0);

                    m_submitDateTimeList.Add(submitDateTime);

                    //var devFee = (ulong)Math.Round(100 / Math.Abs(DevFee.UserPercent));

                    //if (((SubmittedShares - RejectedShares) % devFee) == 0)
                        //SubmitDevFee(address, gasLimit, userGas, SubmittedShares);
                }

                UpdateMinerTimer_Elapsed(this, null);
            }
            catch (AggregateException ex)
            {
                HandleAggregateException(ex);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private BigInteger GetMiningReward()
        {
            var failCount = 0;
            Program.Print("[INFO] Checking mining reward amount from network...");
            while (failCount < 10)
            {
                try
                {
                    if (m_CLM_ContractProgress != null)
                        return m_CLM_ContractProgress.CallDeserializingToObjectAsync<CLM_ContractProgress>().Result.PowReward;
                    
                    return m_getMiningReward.CallAsync<BigInteger>().Result; // including decimals
                }
                catch (Exception) { failCount++; }
            }
            throw new Exception("Failed checking mining reward amount.");
        }

        private void SubmitDevFee(string address, HexBigInteger gasLimit, HexBigInteger userGas, ulong shareNo)
        {
            var success = false;
            var devTransactionID = string.Empty;
            TransactionReceipt devReciept = null;
            try
            {
                var miningReward = GetMiningReward();

                Program.Print(string.Format("[INFO] Transferring dev. fee for successful miner share [{0}]...", shareNo));

                var txInput = new object[] { DevFee.Address, miningReward };

                var txCount = m_web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(address).Result;

                // Commented as gas limit is dynamic in between submissions and confirmations
                //var estimatedGasLimit = m_transferMethod.EstimateGasAsync(from: address,
                //                                                          gas: gasLimit,
                //                                                          value: new HexBigInteger(0),
                //                                                          functionInput: txInput).Result;

                var transaction = m_transferMethod.CreateTransactionInput(from: address,
                                                                          gas: gasLimit /*estimatedGasLimit*/,
                                                                          gasPrice: userGas,
                                                                          value: new HexBigInteger(0),
                                                                          functionInput: txInput);

                var encodedTx = Web3.OfflineTransactionSigner.SignTransaction(privateKey: m_account.PrivateKey,
                                                                              to: m_contract.Address,
                                                                              amount: 0,
                                                                              nonce: txCount.Value,
                                                                              chainId: new HexBigInteger(280),
                                                                              gasPrice: userGas,
                                                                              gasLimit: gasLimit /*estimatedGasLimit*/,
                                                                              data: transaction.Data);

                if (!Web3.OfflineTransactionSigner.VerifyTransaction(encodedTx))
                    throw new Exception("Failed to verify transaction.");

                devTransactionID = m_web3.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + encodedTx).Result;

                if (string.IsNullOrWhiteSpace(devTransactionID)) throw new Exception("Failed to submit dev fee.");

                while (devReciept == null)
                {
                    try
                    {
                        Task.Delay(m_updateInterval / 2).Wait();
                        devReciept = m_web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(devTransactionID).Result;
                    }
                    catch (AggregateException ex)
                    {
                        HandleAggregateException(ex);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                }

                success = (devReciept.Status.Value == 1);

                if (!success) throw new Exception("Failed to submit dev fee.");
                else
                {
                    Program.Print(string.Format("[INFO] Transferred dev fee for successful mint share [{0}] : {1}, block: {2}," +
                                                "\n transaction ID: {3}",
                                                shareNo,
                                                success ? "success" : "failed",
                                                devReciept.BlockNumber.Value,
                                                devReciept.TransactionHash));
                }
            }
            catch (AggregateException ex)
            {
                HandleAggregateException(ex);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void HandleException(Exception ex, string errorPrefix = null)
        {
            var errorMessage = new StringBuilder("[ERROR] ");

            if (!string.IsNullOrWhiteSpace(errorPrefix))
                errorMessage.AppendFormat("{0}: ", errorPrefix);

            errorMessage.Append(ex.Message);

            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                errorMessage.AppendFormat("\n {0}", innerEx.Message);
                innerEx = innerEx.InnerException;
            }
            Program.Print(errorMessage.ToString());
        }

        private void HandleAggregateException(AggregateException ex, string errorPrefix = null)
        {
            var errorMessage = new StringBuilder("[ERROR] ");

            if (!string.IsNullOrWhiteSpace(errorPrefix))
                errorMessage.AppendFormat("{0}: ", errorPrefix);

            errorMessage.Append(ex.Message);

            foreach (var innerException in ex.InnerExceptions)
            {
                errorMessage.AppendFormat("\n {0}", innerException.Message);

                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    errorMessage.AppendFormat("\n  {0}", innerEx.Message);
                    innerEx = innerEx.InnerException;
                }
            }
            Program.Print(errorMessage.ToString());
        }
    }
}